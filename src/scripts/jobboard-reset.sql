--
-- Reset database by dropping and recreating the public schema.
--
DROP SCHEMA IF EXISTS public CASCADE;
CREATE SCHEMA IF NOT EXISTS public;
