-- SASD Health Research Notebook
-- Draft SQLite schema v0.1
-- Status: planning draft, not production-ready
-- Created: 2026-05-25

PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS schema_migrations (
    version INTEGER PRIMARY KEY,
    name TEXT NOT NULL,
    applied_at TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS conditions (
    id TEXT PRIMARY KEY,
    title TEXT NOT NULL,
    normalized_title TEXT NOT NULL,
    condition_type TEXT NOT NULL DEFAULT 'research_topic',
    diagnosis_status TEXT NOT NULL DEFAULT 'unknown',
    priority TEXT NOT NULL DEFAULT 'medium',
    first_noticed_on TEXT NULL,
    diagnosed_on TEXT NULL,
    icd10_code TEXT NULL,
    organ_system TEXT NULL,
    color_marker TEXT NULL,
    summary TEXT NULL,
    notes TEXT NULL,
    is_archived INTEGER NOT NULL DEFAULT 0,
    archived_at TEXT NULL,
    created_at TEXT NOT NULL,
    updated_at TEXT NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_conditions_normalized_title ON conditions(normalized_title);
CREATE INDEX IF NOT EXISTS idx_conditions_type_status ON conditions(condition_type, diagnosis_status);
CREATE INDEX IF NOT EXISTS idx_conditions_archived ON conditions(is_archived);

CREATE TABLE IF NOT EXISTS health_entries (
    id TEXT PRIMARY KEY,
    condition_id TEXT NULL,
    entry_type TEXT NOT NULL DEFAULT 'note',
    title TEXT NOT NULL,
    content TEXT NULL,
    occurred_on TEXT NULL,
    source_confidence TEXT NOT NULL DEFAULT 'unknown',
    importance TEXT NOT NULL DEFAULT 'normal',
    status TEXT NOT NULL DEFAULT 'open',
    created_at TEXT NOT NULL,
    updated_at TEXT NOT NULL,
    FOREIGN KEY (condition_id) REFERENCES conditions(id) ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS idx_health_entries_condition ON health_entries(condition_id);
CREATE INDEX IF NOT EXISTS idx_health_entries_type_date ON health_entries(entry_type, occurred_on);
CREATE INDEX IF NOT EXISTS idx_health_entries_status ON health_entries(status);

CREATE TABLE IF NOT EXISTS symptoms (
    id TEXT PRIMARY KEY,
    name TEXT NOT NULL,
    normalized_name TEXT NOT NULL,
    description TEXT NULL,
    default_scale_min INTEGER NOT NULL DEFAULT 0,
    default_scale_max INTEGER NOT NULL DEFAULT 10,
    created_at TEXT NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_symptoms_normalized_name ON symptoms(normalized_name);

CREATE TABLE IF NOT EXISTS symptom_observations (
    id TEXT PRIMARY KEY,
    condition_id TEXT NOT NULL,
    symptom_id TEXT NOT NULL,
    observed_at TEXT NOT NULL,
    severity INTEGER NULL,
    duration_minutes INTEGER NULL,
    trigger_note TEXT NULL,
    context_note TEXT NULL,
    medication_context TEXT NULL,
    notes TEXT NULL,
    created_at TEXT NOT NULL,
    FOREIGN KEY (condition_id) REFERENCES conditions(id) ON DELETE CASCADE,
    FOREIGN KEY (symptom_id) REFERENCES symptoms(id) ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS idx_symptom_obs_condition_date ON symptom_observations(condition_id, observed_at);
CREATE INDEX IF NOT EXISTS idx_symptom_obs_symptom_date ON symptom_observations(symptom_id, observed_at);

CREATE TABLE IF NOT EXISTS documents (
    id TEXT PRIMARY KEY,
    original_filename TEXT NOT NULL,
    stored_filename TEXT NOT NULL,
    relative_path TEXT NOT NULL,
    mime_type TEXT NULL,
    document_type TEXT NOT NULL DEFAULT 'other',
    title TEXT NOT NULL,
    description TEXT NULL,
    file_size_bytes INTEGER NOT NULL,
    sha256_hash TEXT NOT NULL,
    document_date TEXT NULL,
    imported_at TEXT NOT NULL,
    ocr_status TEXT NOT NULL DEFAULT 'deferred',
    ocr_text TEXT NULL,
    is_sensitive INTEGER NOT NULL DEFAULT 1,
    is_archived INTEGER NOT NULL DEFAULT 0
);

CREATE INDEX IF NOT EXISTS idx_documents_hash ON documents(sha256_hash);
CREATE INDEX IF NOT EXISTS idx_documents_type_date ON documents(document_type, document_date);
CREATE INDEX IF NOT EXISTS idx_documents_archived ON documents(is_archived);

CREATE TABLE IF NOT EXISTS sources (
    id TEXT PRIMARY KEY,
    source_type TEXT NOT NULL DEFAULT 'other',
    title TEXT NOT NULL,
    author_or_provider TEXT NULL,
    url TEXT NULL,
    accessed_on TEXT NULL,
    published_on TEXT NULL,
    reliability_rating TEXT NOT NULL DEFAULT 'unknown',
    notes TEXT NULL,
    created_at TEXT NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_sources_type_rating ON sources(source_type, reliability_rating);
CREATE INDEX IF NOT EXISTS idx_sources_url ON sources(url);

CREATE TABLE IF NOT EXISTS tags (
    id TEXT PRIMARY KEY,
    name TEXT NOT NULL,
    normalized_name TEXT NOT NULL,
    color_marker TEXT NULL,
    created_at TEXT NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_tags_normalized_name ON tags(normalized_name);

CREATE TABLE IF NOT EXISTS synonyms (
    id TEXT PRIMARY KEY,
    condition_id TEXT NULL,
    term TEXT NOT NULL,
    normalized_term TEXT NOT NULL,
    language TEXT NOT NULL DEFAULT 'de',
    note TEXT NULL,
    FOREIGN KEY (condition_id) REFERENCES conditions(id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_synonyms_term ON synonyms(normalized_term);
CREATE INDEX IF NOT EXISTS idx_synonyms_condition ON synonyms(condition_id);

CREATE TABLE IF NOT EXISTS questions (
    id TEXT PRIMARY KEY,
    condition_id TEXT NULL,
    appointment_id TEXT NULL,
    question_text TEXT NOT NULL,
    context_note TEXT NULL,
    target TEXT NOT NULL DEFAULT 'doctor',
    priority TEXT NOT NULL DEFAULT 'normal',
    status TEXT NOT NULL DEFAULT 'open',
    answer_note TEXT NULL,
    created_at TEXT NOT NULL,
    updated_at TEXT NOT NULL,
    FOREIGN KEY (condition_id) REFERENCES conditions(id) ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS idx_questions_condition_status ON questions(condition_id, status);
CREATE INDEX IF NOT EXISTS idx_questions_priority ON questions(priority);

CREATE TABLE IF NOT EXISTS condition_documents (
    condition_id TEXT NOT NULL,
    document_id TEXT NOT NULL,
    relation_note TEXT NULL,
    PRIMARY KEY (condition_id, document_id),
    FOREIGN KEY (condition_id) REFERENCES conditions(id) ON DELETE CASCADE,
    FOREIGN KEY (document_id) REFERENCES documents(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS condition_sources (
    condition_id TEXT NOT NULL,
    source_id TEXT NOT NULL,
    relation_note TEXT NULL,
    PRIMARY KEY (condition_id, source_id),
    FOREIGN KEY (condition_id) REFERENCES conditions(id) ON DELETE CASCADE,
    FOREIGN KEY (source_id) REFERENCES sources(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS entry_documents (
    entry_id TEXT NOT NULL,
    document_id TEXT NOT NULL,
    PRIMARY KEY (entry_id, document_id),
    FOREIGN KEY (entry_id) REFERENCES health_entries(id) ON DELETE CASCADE,
    FOREIGN KEY (document_id) REFERENCES documents(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS entry_sources (
    entry_id TEXT NOT NULL,
    source_id TEXT NOT NULL,
    PRIMARY KEY (entry_id, source_id),
    FOREIGN KEY (entry_id) REFERENCES health_entries(id) ON DELETE CASCADE,
    FOREIGN KEY (source_id) REFERENCES sources(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS condition_tags (
    condition_id TEXT NOT NULL,
    tag_id TEXT NOT NULL,
    PRIMARY KEY (condition_id, tag_id),
    FOREIGN KEY (condition_id) REFERENCES conditions(id) ON DELETE CASCADE,
    FOREIGN KEY (tag_id) REFERENCES tags(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS entry_tags (
    entry_id TEXT NOT NULL,
    tag_id TEXT NOT NULL,
    PRIMARY KEY (entry_id, tag_id),
    FOREIGN KEY (entry_id) REFERENCES health_entries(id) ON DELETE CASCADE,
    FOREIGN KEY (tag_id) REFERENCES tags(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS document_tags (
    document_id TEXT NOT NULL,
    tag_id TEXT NOT NULL,
    PRIMARY KEY (document_id, tag_id),
    FOREIGN KEY (document_id) REFERENCES documents(id) ON DELETE CASCADE,
    FOREIGN KEY (tag_id) REFERENCES tags(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS timeline_events (
    id TEXT PRIMARY KEY,
    condition_id TEXT NULL,
    event_type TEXT NOT NULL,
    event_at TEXT NOT NULL,
    title TEXT NOT NULL,
    description TEXT NULL,
    related_entity_type TEXT NULL,
    related_entity_id TEXT NULL,
    created_at TEXT NOT NULL,
    FOREIGN KEY (condition_id) REFERENCES conditions(id) ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS idx_timeline_condition_date ON timeline_events(condition_id, event_at);
CREATE INDEX IF NOT EXISTS idx_timeline_type_date ON timeline_events(event_type, event_at);

CREATE TABLE IF NOT EXISTS wizard_drafts (
    id TEXT PRIMARY KEY,
    wizard_type TEXT NOT NULL,
    current_step INTEGER NOT NULL DEFAULT 1,
    draft_json TEXT NOT NULL,
    created_at TEXT NOT NULL,
    updated_at TEXT NOT NULL,
    expires_at TEXT NULL,
    is_completed INTEGER NOT NULL DEFAULT 0
);

CREATE INDEX IF NOT EXISTS idx_wizard_drafts_type_completed ON wizard_drafts(wizard_type, is_completed);

CREATE TABLE IF NOT EXISTS audit_log_entries (
    id TEXT PRIMARY KEY,
    event_at TEXT NOT NULL,
    action TEXT NOT NULL,
    entity_type TEXT NOT NULL,
    entity_id TEXT NULL,
    summary TEXT NOT NULL,
    technical_details TEXT NULL
);

CREATE INDEX IF NOT EXISTS idx_audit_event_at ON audit_log_entries(event_at);
CREATE INDEX IF NOT EXISTS idx_audit_entity ON audit_log_entries(entity_type, entity_id);

-- Full text search draft.
-- In production, triggers or explicit index services should keep this synchronized.
CREATE VIRTUAL TABLE IF NOT EXISTS search_index USING fts5(
    entity_type,
    entity_id UNINDEXED,
    title,
    content,
    tags,
    synonyms,
    tokenize = 'unicode61 remove_diacritics 2'
);
