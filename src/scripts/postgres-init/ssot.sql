CREATE DATABASE ssot;

CREATE ROLE ssot_readonly NOLOGIN;
GRANT CONNECT ON DATABASE ssot TO ssot_readonly;
GRANT USAGE ON SCHEMA public TO ssot_readonly;
GRANT SELECT ON ALL TABLES IN SCHEMA public TO ssot_readonly;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT ON TABLES TO ssot_readonly;

CREATE ROLE ssot_lmmu LOGIN PASSWORD 'ssot_lmmu';
GRANT CONNECT ON DATABASE ssot TO ssot_lmmu;
GRANT USAGE ON SCHEMA public TO ssot_lmmu;
GRANT SELECT ON ALL TABLES IN SCHEMA public TO ssot_lmmu;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT ON TABLES TO ssot_lmmu;

-- https://postgrest.org/en/stable/schema_cache.html#schema-reloading
-- Create an event trigger function
CREATE OR REPLACE FUNCTION public.pgrst_watch() RETURNS event_trigger
  LANGUAGE plpgsql
  AS $$
BEGIN
  NOTIFY pgrst, 'reload schema';
END;
$$;

-- This event trigger will fire after every ddl_command_end event
CREATE EVENT TRIGGER pgrst_watch
  ON ddl_command_end
  EXECUTE PROCEDURE public.pgrst_watch();

-- Turn on case-insensitive text extension
CREATE EXTENSION citext;
