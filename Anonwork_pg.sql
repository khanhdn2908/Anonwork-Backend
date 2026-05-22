-- ============================================================
--  ANONWORK DATABASE SCHEMA
--  LOCAL POSTGRESQL VERSION
-- ============================================================

CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- ============================================================
-- USERS
-- ============================================================

CREATE TABLE users (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

  username VARCHAR(50) NOT NULL UNIQUE,

  email VARCHAR(255) NOT NULL UNIQUE,

  password_hash TEXT NOT NULL,

  avatar_url TEXT NULL,

  bio TEXT NULL,

  anon_alias VARCHAR(80) NOT NULL UNIQUE,

  is_anon_default BOOLEAN NOT NULL DEFAULT FALSE,

  role VARCHAR(20) NOT NULL DEFAULT 'student'
    CHECK (role IN ('student','teacher','moderator','admin')),

  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

  updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_users_username ON users(username);

-- ============================================================
-- SUBJECTS
-- ============================================================

CREATE TABLE subjects (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

  name VARCHAR(100) NOT NULL,

  slug VARCHAR(100) NOT NULL UNIQUE,

  icon_emoji VARCHAR(10),

  post_count INT NOT NULL DEFAULT 0,

  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ============================================================
-- POSTS
-- ============================================================

CREATE TABLE posts (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

  author_id UUID NOT NULL
    REFERENCES users(id)
    ON DELETE CASCADE,

  subject_id UUID NOT NULL
    REFERENCES subjects(id)
    ON DELETE RESTRICT,

  is_anonymous BOOLEAN NOT NULL DEFAULT FALSE,

  title VARCHAR(255) NOT NULL,

  content TEXT NOT NULL,

  search_vector tsvector,

  upvotes INT NOT NULL DEFAULT 0,

  comments_count INT NOT NULL DEFAULT 0,

  view_count INT NOT NULL DEFAULT 0,

  status VARCHAR(20) NOT NULL DEFAULT 'active'
    CHECK (status IN ('active','pending','removed')),

  deleted_at TIMESTAMPTZ NULL,

  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

  updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE post_images (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

  post_id UUID NOT NULL
    REFERENCES posts(id)
    ON DELETE CASCADE,

  image_url TEXT NOT NULL,

  display_order INT NOT NULL DEFAULT 0,

  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_posts_author
  ON posts(author_id);

CREATE INDEX idx_posts_subject
  ON posts(subject_id, created_at DESC);

CREATE INDEX idx_posts_feed
  ON posts(status, created_at DESC);

CREATE INDEX idx_posts_search
  ON posts USING GIN(search_vector);

CREATE INDEX idx_post_images_post
  ON post_images(post_id);

-- ============================================================
-- SEARCH VECTOR TRIGGER
-- ============================================================

CREATE OR REPLACE FUNCTION posts_search_vector_update()
RETURNS trigger AS $$
BEGIN
  NEW.search_vector :=
    to_tsvector(
      'english',
      coalesce(NEW.title, '') || ' ' ||
      coalesce(NEW.content, '')
    );

  RETURN NEW;
END
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_posts_search_vector
BEFORE INSERT OR UPDATE
ON posts
FOR EACH ROW
EXECUTE FUNCTION posts_search_vector_update();

-- ============================================================
-- POST TAGS
-- ============================================================

CREATE TABLE post_tags (
  post_id UUID NOT NULL
    REFERENCES posts(id)
    ON DELETE CASCADE,

  tag VARCHAR(50) NOT NULL,

  PRIMARY KEY(post_id, tag)
);

CREATE INDEX idx_post_tags_tag
  ON post_tags(tag);

-- ============================================================
-- COMMENTS
-- ============================================================

CREATE TABLE comments (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

  post_id UUID NOT NULL
    REFERENCES posts(id)
    ON DELETE CASCADE,

  author_id UUID NOT NULL
    REFERENCES users(id)
    ON DELETE CASCADE,

  parent_id UUID NULL
    REFERENCES comments(id)
    ON DELETE CASCADE,

  is_anonymous BOOLEAN NOT NULL DEFAULT FALSE,

  content TEXT NOT NULL,

  upvotes INT NOT NULL DEFAULT 0,

  depth INT NOT NULL DEFAULT 0
    CHECK(depth >= 0 AND depth <= 3),

  is_deleted BOOLEAN NOT NULL DEFAULT FALSE,

  deleted_at TIMESTAMPTZ NULL,

  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

  updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_comments_post
  ON comments(post_id);

CREATE INDEX idx_comments_post_created
  ON comments(post_id, created_at);

CREATE INDEX idx_comments_parent
  ON comments(parent_id);

-- ============================================================
-- VOTES
-- ============================================================

CREATE TABLE votes (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

  user_id UUID NOT NULL
    REFERENCES users(id)
    ON DELETE CASCADE,

  target_id UUID NOT NULL,

  target_type VARCHAR(10) NOT NULL
    CHECK(target_type IN ('post','comment')),

  vote_type VARCHAR(5) NOT NULL DEFAULT 'up'
    CHECK(vote_type IN ('up','down')),

  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

  UNIQUE(user_id, target_id, target_type)
);

CREATE INDEX idx_votes_target
  ON votes(target_id, target_type);

-- ============================================================
-- FOLLOWS
-- ============================================================

CREATE TABLE follows (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

  follower_id UUID NOT NULL
    REFERENCES users(id)
    ON DELETE CASCADE,

  following_id UUID NOT NULL
    REFERENCES users(id)
    ON DELETE CASCADE,

  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

  CONSTRAINT uq_follows
    UNIQUE(follower_id, following_id),

  CONSTRAINT chk_follows_no_self
    CHECK(follower_id <> following_id)
);

CREATE INDEX idx_follows_following
  ON follows(following_id);

-- ============================================================
-- BOOKMARKS
-- ============================================================

CREATE TABLE bookmarks (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

  user_id UUID NOT NULL
    REFERENCES users(id)
    ON DELETE CASCADE,

  post_id UUID NOT NULL
    REFERENCES posts(id)
    ON DELETE CASCADE,

  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

  UNIQUE(user_id, post_id)
);

CREATE INDEX idx_bookmarks_user
  ON bookmarks(user_id, created_at DESC);

-- ============================================================
-- NOTIFICATIONS
-- ============================================================

CREATE TABLE notifications (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

  user_id UUID NOT NULL
    REFERENCES users(id)
    ON DELETE CASCADE,

  actor_id UUID NULL
    REFERENCES users(id)
    ON DELETE SET NULL,

  type VARCHAR(20) NOT NULL
    CHECK(type IN (
      'new_comment',
      'upvote',
      'new_follower',
      'mention',
      'system',
      'ranking'
    )),

  ref_id UUID NULL,

  is_read BOOLEAN NOT NULL DEFAULT FALSE,

  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_notifications_user
  ON notifications(user_id, is_read, created_at DESC);

-- ============================================================
-- REPORTS
-- ============================================================

CREATE TABLE reports (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

  reporter_id UUID NOT NULL
    REFERENCES users(id)
    ON DELETE CASCADE,

  target_id UUID NOT NULL,

  target_type VARCHAR(10) NOT NULL
    CHECK(target_type IN ('post','comment')),

  reason VARCHAR(500) NOT NULL,

  status VARCHAR(20) NOT NULL DEFAULT 'pending'
    CHECK(status IN ('pending','resolved','dismissed')),

  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

  updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_reports_status
  ON reports(status);

-- ============================================================
-- CONVERSATIONS
-- ============================================================

CREATE TABLE conversations (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

  is_group BOOLEAN NOT NULL DEFAULT FALSE,

  title VARCHAR(100) NULL,

  avatar_url TEXT NULL,

  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

  updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ============================================================
-- CONVERSATION MEMBERS
-- ============================================================

CREATE TABLE conversation_members (
  conversation_id UUID NOT NULL
    REFERENCES conversations(id)
    ON DELETE CASCADE,

  user_id UUID NOT NULL
    REFERENCES users(id)
    ON DELETE CASCADE,

  joined_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

  last_read_at TIMESTAMPTZ NULL,

  PRIMARY KEY(conversation_id, user_id)
);

-- ============================================================
-- MESSAGES
-- ============================================================

CREATE TABLE messages (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

  conversation_id UUID NOT NULL
    REFERENCES conversations(id)
    ON DELETE CASCADE,

  sender_id UUID NOT NULL
    REFERENCES users(id)
    ON DELETE CASCADE,

  content TEXT NOT NULL,

  is_deleted BOOLEAN NOT NULL DEFAULT FALSE,

  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_messages_conversation
  ON messages(conversation_id, created_at DESC);

-- ============================================================
-- UPDATED_AT TRIGGER
-- ============================================================

CREATE OR REPLACE FUNCTION set_updated_at()
RETURNS TRIGGER AS $$
BEGIN
  NEW.updated_at = NOW();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_users_updated_at
BEFORE UPDATE ON users
FOR EACH ROW
EXECUTE FUNCTION set_updated_at();

CREATE TRIGGER trg_posts_updated_at
BEFORE UPDATE ON posts
FOR EACH ROW
EXECUTE FUNCTION set_updated_at();

CREATE TRIGGER trg_comments_updated_at
BEFORE UPDATE ON comments
FOR EACH ROW
EXECUTE FUNCTION set_updated_at();

CREATE TRIGGER trg_reports_updated_at
BEFORE UPDATE ON reports
FOR EACH ROW
EXECUTE FUNCTION set_updated_at();

CREATE TRIGGER trg_conversations_updated_at
BEFORE UPDATE ON conversations
FOR EACH ROW
EXECUTE FUNCTION set_updated_at();


-- ============================================================
-- SAMPLE DATA
-- ============================================================

-- USERS

INSERT INTO users (
    username,
    email,
    password_hash,
    anon_alias,
    bio
)
VALUES
(
    'khanh',
    'khanh@gmail.com',
    'hashed_password',
    'SilentWhale',
    'Backend developer'
),
(
    'linh',
    'linh@gmail.com',
    'hashed_password',
    'MoonCat',
    'UI/UX Designer'
),
(
    'an',
    'an@gmail.com',
    'hashed_password',
    'DarkFox',
    'Game developer'
);

-- SUBJECTS

INSERT INTO subjects (
    name,
    slug,
    icon_emoji
)
VALUES
(
    'Programming',
    'programming',
    '💻'
),
(
    'Gaming',
    'gaming',
    '🎮'
),
(
    'University',
    'university',
    '📚'
);

-- POSTS

INSERT INTO posts (
    author_id,
    subject_id,
    title,
    content,
    is_anonymous
)
VALUES
(
    (SELECT id FROM users WHERE username = 'khanh'),
    (SELECT id FROM subjects WHERE slug = 'programming'),
    'How to learn Clean Architecture?',
    'I am learning Clean Architecture with .NET and PostgreSQL.',
    FALSE
),
(
    (SELECT id FROM users WHERE username = 'linh'),
    (SELECT id FROM subjects WHERE slug = 'gaming'),
    'Best horror games in 2026?',
    'Can anyone recommend some horror games?',
    TRUE
),
(
    (SELECT id FROM users WHERE username = 'an'),
    (SELECT id FROM subjects WHERE slug = 'university'),
    'How to survive final exams?',
    'Need tips for managing study schedule.',
    TRUE
);

-- COMMENTS

INSERT INTO comments (
    post_id,
    author_id,
    content
)
VALUES
(
    (SELECT id FROM posts LIMIT 1),
    (SELECT id FROM users WHERE username = 'linh'),
    'You should start with Domain Driven Design first.'
),
(
    (SELECT id FROM posts LIMIT 1),
    (SELECT id FROM users WHERE username = 'an'),
    'Try separating Application and Infrastructure layers.'
);

-- FOLLOWS

INSERT INTO follows (
    follower_id,
    following_id
)
VALUES
(
    (SELECT id FROM users WHERE username = 'khanh'),
    (SELECT id FROM users WHERE username = 'linh')
),
(
    (SELECT id FROM users WHERE username = 'linh'),
    (SELECT id FROM users WHERE username = 'an')
);

-- BOOKMARKS

INSERT INTO bookmarks (
    user_id,
    post_id
)
VALUES
(
    (SELECT id FROM users WHERE username = 'khanh'),
    (SELECT id FROM posts LIMIT 1)
);

-- CONVERSATIONS

INSERT INTO conversations (
    is_group,
    title
)
VALUES
(
    FALSE,
    'Private Chat'
);

-- CONVERSATION MEMBERS

INSERT INTO conversation_members (
    conversation_id,
    user_id
)
VALUES
(
    (SELECT id FROM conversations LIMIT 1),
    (SELECT id FROM users WHERE username = 'khanh')
),
(
    (SELECT id FROM conversations LIMIT 1),
    (SELECT id FROM users WHERE username = 'linh')
);

-- MESSAGES

INSERT INTO messages (
    conversation_id,
    sender_id,
    content
)
VALUES
(
    (SELECT id FROM conversations LIMIT 1),
    (SELECT id FROM users WHERE username = 'khanh'),
    'Hello Linh!'
),
(
    (SELECT id FROM conversations LIMIT 1),
    (SELECT id FROM users WHERE username = 'linh'),
    'Hi Khanh!'
);

-- NOTIFICATIONS

INSERT INTO notifications (
    user_id,
    actor_id,
    type
)
VALUES
(
    (SELECT id FROM users WHERE username = 'khanh'),
    (SELECT id FROM users WHERE username = 'linh'),
    'new_follower'
);

-- REPORTS

INSERT INTO reports (
    reporter_id,
    target_id,
    target_type,
    reason
)
VALUES
(
    (SELECT id FROM users WHERE username = 'linh'),
    (SELECT id FROM posts LIMIT 1),
    'post',
    'Spam content'
);