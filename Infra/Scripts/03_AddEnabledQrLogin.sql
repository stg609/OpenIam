ALTER TABLE sysinfo ADD enabledqrexternallogins text[] NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20210405074941_AddEnableQrLoginProp', '3.1.10');

