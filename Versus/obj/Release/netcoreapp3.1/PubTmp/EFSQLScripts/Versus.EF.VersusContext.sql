CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);


DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527042525_init') THEN
    CREATE TABLE "Exercise" (
        "Id" uuid NOT NULL,
        "Wins" integer NOT NULL,
        "Losses" integer NOT NULL,
        "HighScore" integer NOT NULL,
        CONSTRAINT "PK_Exercise" PRIMARY KEY ("Id")
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527042525_init') THEN
    CREATE TABLE "Notifications" (
        "Id" uuid NOT NULL,
        "Mon" boolean NOT NULL,
        "Tue" boolean NOT NULL,
        "Wed" boolean NOT NULL,
        "Thu" boolean NOT NULL,
        "Fri" boolean NOT NULL,
        "Sat" boolean NOT NULL,
        "Sun" boolean NOT NULL,
        CONSTRAINT "PK_Notifications" PRIMARY KEY ("Id")
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527042525_init') THEN
    CREATE TABLE "Vip" (
        "Id" uuid NOT NULL,
        "Begin" timestamp without time zone NOT NULL,
        "Duration" integer NOT NULL,
        CONSTRAINT "PK_Vip" PRIMARY KEY ("Id")
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527042525_init') THEN
    CREATE TABLE "Exercises" (
        "Id" uuid NOT NULL,
        "PushUpsId" uuid NOT NULL,
        "PullUpsId" uuid NOT NULL,
        "AbsId" uuid NOT NULL,
        "SquatsId" uuid NOT NULL,
        CONSTRAINT "PK_Exercises" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Exercises_Exercise_AbsId" FOREIGN KEY ("AbsId") REFERENCES "Exercise" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_Exercises_Exercise_PullUpsId" FOREIGN KEY ("PullUpsId") REFERENCES "Exercise" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_Exercises_Exercise_PushUpsId" FOREIGN KEY ("PushUpsId") REFERENCES "Exercise" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_Exercises_Exercise_SquatsId" FOREIGN KEY ("SquatsId") REFERENCES "Exercise" ("Id") ON DELETE CASCADE
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527042525_init') THEN
    CREATE TABLE "Settings" (
        "Id" uuid NOT NULL,
        "Language" boolean NOT NULL,
        "Sound" boolean NOT NULL,
        "Invites" boolean NOT NULL,
        "NotificationsId" uuid NOT NULL,
        CONSTRAINT "PK_Settings" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Settings_Notifications_NotificationsId" FOREIGN KEY ("NotificationsId") REFERENCES "Notifications" ("Id") ON DELETE CASCADE
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527042525_init') THEN
    CREATE TABLE "User" (
        "Id" uuid NOT NULL,
        "Email" text NULL,
        "Name" text NULL,
        "Country" text NULL,
        "SettingsId" uuid NOT NULL,
        "VipId" uuid NOT NULL,
        "ExercisesId" uuid NOT NULL,
        CONSTRAINT "PK_User" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_User_Exercises_ExercisesId" FOREIGN KEY ("ExercisesId") REFERENCES "Exercises" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_User_Settings_SettingsId" FOREIGN KEY ("SettingsId") REFERENCES "Settings" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_User_Vip_VipId" FOREIGN KEY ("VipId") REFERENCES "Vip" ("Id") ON DELETE CASCADE
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527042525_init') THEN
    CREATE INDEX "IX_Exercises_AbsId" ON "Exercises" ("AbsId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527042525_init') THEN
    CREATE INDEX "IX_Exercises_PullUpsId" ON "Exercises" ("PullUpsId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527042525_init') THEN
    CREATE INDEX "IX_Exercises_PushUpsId" ON "Exercises" ("PushUpsId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527042525_init') THEN
    CREATE INDEX "IX_Exercises_SquatsId" ON "Exercises" ("SquatsId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527042525_init') THEN
    CREATE INDEX "IX_Settings_NotificationsId" ON "Settings" ("NotificationsId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527042525_init') THEN
    CREATE UNIQUE INDEX "IX_User_ExercisesId" ON "User" ("ExercisesId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527042525_init') THEN
    CREATE UNIQUE INDEX "IX_User_SettingsId" ON "User" ("SettingsId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527042525_init') THEN
    CREATE UNIQUE INDEX "IX_User_VipId" ON "User" ("VipId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527042525_init') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20200527042525_init', '3.1.4');
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527043934_changeKeys') THEN
    ALTER TABLE "User" DROP CONSTRAINT "FK_User_Exercises_ExercisesId";
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527043934_changeKeys') THEN
    ALTER TABLE "User" DROP CONSTRAINT "FK_User_Settings_SettingsId";
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527043934_changeKeys') THEN
    ALTER TABLE "User" DROP CONSTRAINT "FK_User_Vip_VipId";
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527043934_changeKeys') THEN
    DROP INDEX "IX_User_ExercisesId";
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527043934_changeKeys') THEN
    DROP INDEX "IX_User_SettingsId";
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527043934_changeKeys') THEN
    DROP INDEX "IX_User_VipId";
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527043934_changeKeys') THEN
    ALTER TABLE "User" DROP COLUMN "ExercisesId";
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527043934_changeKeys') THEN
    ALTER TABLE "User" DROP COLUMN "SettingsId";
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527043934_changeKeys') THEN
    ALTER TABLE "User" DROP COLUMN "VipId";
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527043934_changeKeys') THEN
    ALTER TABLE "Vip" ADD "UserId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527043934_changeKeys') THEN
    ALTER TABLE "Settings" ADD "UserId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527043934_changeKeys') THEN
    ALTER TABLE "Exercises" ADD "UserId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527043934_changeKeys') THEN
    CREATE UNIQUE INDEX "IX_Vip_UserId" ON "Vip" ("UserId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527043934_changeKeys') THEN
    CREATE UNIQUE INDEX "IX_Settings_UserId" ON "Settings" ("UserId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527043934_changeKeys') THEN
    CREATE UNIQUE INDEX "IX_Exercises_UserId" ON "Exercises" ("UserId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527043934_changeKeys') THEN
    ALTER TABLE "Exercises" ADD CONSTRAINT "FK_Exercises_User_UserId" FOREIGN KEY ("UserId") REFERENCES "User" ("Id") ON DELETE CASCADE;
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527043934_changeKeys') THEN
    ALTER TABLE "Settings" ADD CONSTRAINT "FK_Settings_User_UserId" FOREIGN KEY ("UserId") REFERENCES "User" ("Id") ON DELETE CASCADE;
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527043934_changeKeys') THEN
    ALTER TABLE "Vip" ADD CONSTRAINT "FK_Vip_User_UserId" FOREIGN KEY ("UserId") REFERENCES "User" ("Id") ON DELETE CASCADE;
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200527043934_changeKeys') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20200527043934_changeKeys', '3.1.4');
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200529082248_usetToken') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20200529082248_usetToken', '3.1.4');
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200529082839_userToken') THEN
    ALTER TABLE "User" ADD "Token" text NULL;
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20200529082839_userToken') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20200529082839_userToken', '3.1.4');
    END IF;
END $$;
