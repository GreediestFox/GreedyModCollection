-- Skriptorium schema (D1 / SQLite)
CREATE TABLE IF NOT EXISTS documents (
  id TEXT PRIMARY KEY,
  type TEXT NOT NULL CHECK(type IN ('letter','book','tablet')),
  title TEXT NOT NULL DEFAULT '',
  finalized INTEGER NOT NULL DEFAULT 0,
  sealed INTEGER NOT NULL DEFAULT 0,
  password_hash TEXT,
  cipher_map TEXT,
  encrypted_text TEXT,
  cover_author TEXT,
  cover_color TEXT,
  cover_text_color TEXT,
  ticket TEXT,
  created_at TEXT NOT NULL DEFAULT (datetime('now')),
  updated_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS blocks (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  document_id TEXT NOT NULL REFERENCES documents(id) ON DELETE CASCADE,
  seq INTEGER NOT NULL,
  html TEXT NOT NULL,
  created_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE INDEX IF NOT EXISTS idx_blocks_document ON blocks(document_id, seq);
CREATE INDEX IF NOT EXISTS idx_documents_ticket ON documents(ticket);
