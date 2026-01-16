-- ============================================
-- Authentication Service - PostgreSQL Schema
-- Database: production
-- Schema: dashboard
-- ============================================

-- Create schema
CREATE SCHEMA IF NOT EXISTS dashboard;

-- ============================================
-- Table: users
-- ============================================
CREATE TABLE dashboard.users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    failed_login_attempts INTEGER NOT NULL DEFAULT 0,
    locked_until TIMESTAMP NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Indexes for users table
CREATE INDEX idx_users_username ON dashboard.users(username);
CREATE INDEX idx_users_email ON dashboard.users(email);

-- ============================================
-- Table: refresh_tokens
-- ============================================
CREATE TABLE dashboard.refresh_tokens (
    id SERIAL PRIMARY KEY,
    token TEXT NOT NULL UNIQUE,
    user_id INTEGER NOT NULL,
    expires_at TIMESTAMP NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    revoked_at TIMESTAMP NULL,
    
    -- Foreign key
    CONSTRAINT fk_refresh_tokens_user_id 
        FOREIGN KEY (user_id) 
        REFERENCES dashboard.users(id) 
        ON DELETE CASCADE
);

-- Indexes for refresh_tokens table
CREATE INDEX idx_refresh_tokens_token ON dashboard.refresh_tokens(token);
CREATE INDEX idx_refresh_tokens_user_id ON dashboard.refresh_tokens(user_id);

-- ============================================
-- Trigger: Auto-update updated_at timestamp
-- ============================================
CREATE OR REPLACE FUNCTION dashboard.update_updated_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_update_users_updated_at
    BEFORE UPDATE ON dashboard.users
    FOR EACH ROW
    EXECUTE FUNCTION dashboard.update_updated_at();

-- ============================================
-- Verify tables created
-- ============================================
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'dashboard';

