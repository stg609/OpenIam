ALTER TABLE "AspNetUsers" ADD avatar text NULL;

ALTER TABLE "AspNetUsers" ADD cover text NULL;

ALTER TABLE "AspNetUsers" ADD github text NULL;

ALTER TABLE "AspNetUsers" ADD motto text NULL;

ALTER TABLE "AspNetUsers" ADD note text NULL;

ALTER TABLE "AspNetUsers" ADD sinaweibo text NULL;

ALTER TABLE "AspNetUsers" ADD twitter text NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20210302033755_AddMoreUserInfo', '3.1.10');

