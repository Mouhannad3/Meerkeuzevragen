DROP DATABASE IF EXISTS Meerkeuzevragen;
GO

CREATE DATABASE Meerkeuzevragen;
GO

USE Meerkeuzevragen;
GO

CREATE TABLE Onderwerp (
    onderwerp_id INT IDENTITY(1,1) NOT NULL,
    naam NVARCHAR(100) NOT NULL,

    CONSTRAINT pk_onderwerp PRIMARY KEY CLUSTERED (onderwerp_id ASC),
    CONSTRAINT uq_onderwerp_naam UNIQUE (naam)
);
GO

CREATE TABLE Vraag (
    vraag_id INT IDENTITY(1,1) NOT NULL,
    onderwerp_id INT NOT NULL,
    tekst NVARCHAR(MAX) NOT NULL,
    is_beschikbaar BIT NOT NULL DEFAULT 1,

    CONSTRAINT pk_vraag PRIMARY KEY CLUSTERED (vraag_id ASC),
    CONSTRAINT fk_vraag_onderwerp FOREIGN KEY (onderwerp_id) REFERENCES Onderwerp(onderwerp_id)
);
GO

CREATE TABLE Antwoord (
    antwoord_id INT IDENTITY(1,1) NOT NULL,
    vraag_id INT NOT NULL,
    tekst NVARCHAR(MAX) NOT NULL,
    is_correct BIT NOT NULL,

    CONSTRAINT pk_antwoord PRIMARY KEY CLUSTERED (antwoord_id ASC),
    CONSTRAINT fk_antwoord_vraag FOREIGN KEY (vraag_id) REFERENCES Vraag(vraag_id)
);
GO

CREATE TABLE Test (
    test_id INT IDENTITY(1,1) NOT NULL,
    onderwerp_id INT NOT NULL,
    naam NVARCHAR(100) NOT NULL,
    aangemaakt_op DATETIME2 NOT NULL,
    aantal_antwoorden_per_vraag INT NOT NULL,

    CONSTRAINT pk_test PRIMARY KEY CLUSTERED (test_id ASC),
    CONSTRAINT fk_test_onderwerp FOREIGN KEY (onderwerp_id) REFERENCES Onderwerp(onderwerp_id),
    CONSTRAINT ck_test_aantal_antwoorden_per_vraag CHECK (aantal_antwoorden_per_vraag >= 2)
);
GO

CREATE TABLE TestVraag (
    test_vraag_id INT IDENTITY(1,1) NOT NULL,
    test_id INT NOT NULL,
    vraag_id INT NOT NULL,
    volgorde INT NOT NULL,

    CONSTRAINT pk_testvraag PRIMARY KEY CLUSTERED (test_vraag_id ASC),
    CONSTRAINT fk_testvraag_test FOREIGN KEY (test_id) REFERENCES Test(test_id),
    CONSTRAINT fk_testvraag_vraag FOREIGN KEY (vraag_id) REFERENCES Vraag(vraag_id),
    CONSTRAINT ck_testvraag_volgorde CHECK (volgorde > 0),
    CONSTRAINT uq_testvraag_test_vraag UNIQUE (test_id, vraag_id),
    CONSTRAINT uq_testvraag_test_volgorde UNIQUE (test_id, volgorde)
);
GO

CREATE TABLE TestVraagAntwoord (
    test_vraag_antwoord_id INT IDENTITY(1,1) NOT NULL,
    test_vraag_id INT NOT NULL,
    antwoord_id INT NOT NULL,
    letter CHAR(1) NOT NULL,
    volgorde INT NOT NULL,

    CONSTRAINT pk_testvraagantwoord PRIMARY KEY CLUSTERED (test_vraag_antwoord_id ASC),
    CONSTRAINT fk_testvraagantwoord_testvraag FOREIGN KEY (test_vraag_id) REFERENCES TestVraag(test_vraag_id),
    CONSTRAINT fk_testvraagantwoord_antwoord FOREIGN KEY (antwoord_id) REFERENCES Antwoord(antwoord_id),
    CONSTRAINT ck_testvraagantwoord_letter CHECK (letter LIKE '[A-Z]'),
    CONSTRAINT ck_testvraagantwoord_volgorde CHECK (volgorde > 0),
    CONSTRAINT uq_testvraagantwoord_testvraag_letter UNIQUE (test_vraag_id, letter),
    CONSTRAINT uq_testvraagantwoord_testvraag_antwoord UNIQUE (test_vraag_id, antwoord_id),
    CONSTRAINT uq_testvraagantwoord_testvraag_volgorde UNIQUE (test_vraag_id, volgorde)
);
GO

CREATE TABLE Gebruiker (
    gebruiker_id INT IDENTITY(1,1) NOT NULL,
    naam NVARCHAR(100) NOT NULL,

    CONSTRAINT pk_gebruiker PRIMARY KEY CLUSTERED (gebruiker_id ASC)
);
GO

CREATE TABLE TestResultaat (
    test_resultaat_id INT IDENTITY(1,1) NOT NULL,
    test_id INT NOT NULL,
    gebruiker_id INT NOT NULL,
    score INT NOT NULL,
    totaal_aantal_vragen INT NOT NULL,
    uitgevoerd_op DATETIME2 NOT NULL,

    CONSTRAINT pk_testresultaat PRIMARY KEY CLUSTERED (test_resultaat_id ASC),
    CONSTRAINT fk_testresultaat_test FOREIGN KEY (test_id) REFERENCES Test(test_id),
    CONSTRAINT fk_testresultaat_gebruiker FOREIGN KEY (gebruiker_id) REFERENCES Gebruiker(gebruiker_id),
    CONSTRAINT ck_testresultaat_score CHECK (score >= 0),
    CONSTRAINT ck_testresultaat_totaal_aantal_vragen CHECK (totaal_aantal_vragen > 0),
    CONSTRAINT ck_testresultaat_score_max CHECK (score <= totaal_aantal_vragen)
);
GO

CREATE TABLE GebruikerAntwoord (
    gebruiker_antwoord_id INT IDENTITY(1,1) NOT NULL,
    test_resultaat_id INT NOT NULL,
    test_vraag_id INT NOT NULL,
    gekozen_letter CHAR(1) NOT NULL,
    is_correct BIT NOT NULL,

    CONSTRAINT pk_gebruikerantwoord PRIMARY KEY CLUSTERED (gebruiker_antwoord_id ASC),
    CONSTRAINT fk_gebruikerantwoord_testresultaat FOREIGN KEY (test_resultaat_id) REFERENCES TestResultaat(test_resultaat_id),
    CONSTRAINT fk_gebruikerantwoord_testvraag FOREIGN KEY (test_vraag_id) REFERENCES TestVraag(test_vraag_id),
    CONSTRAINT ck_gebruikerantwoord_gekozen_letter CHECK (gekozen_letter LIKE '[A-Z]'),
    CONSTRAINT uq_gebruikerantwoord_resultaat_testvraag UNIQUE (test_resultaat_id, test_vraag_id)
);
GO