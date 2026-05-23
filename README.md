# Meerkeuzevragen Applicatie

## Doel van het project

Dit project is een WPF-desktopapplicatie voor het beheren, samenstellen, uitvoeren en verbeteren van meerkeuzetesten.

Met de applicatie kan de gebruiker:

- onderwerpen toevoegen;
- vragen importeren uit tekstbestanden;
- nieuwe vragen toevoegen;
- vragen niet meer beschikbaar stellen;
- testen samenstellen op basis van onderwerp en aantal vragen;
- testen exporteren naar een txt-bestand;
- testen uitvoeren;
- scores berekenen;
- feedback tonen bij fout beantwoorde vragen;
- bulkresultaten verwerken.

---

## Gebruikte technologieën

- C#
- WPF
- SQL Server
- ADO.NET
- xUnit
- GitHub

---

## Architectuur

De applicatie gebruikt een 3-lagen architectuur waarbij de businesslaag centraal staat.

```text
UI → BL → DL → Database

De UI-laag bevat de WPF-schermen.
De BL-laag bevat de domeinklassen, interfaces, managers en exceptions.
De DL-laag implementeert de interfaces uit de BL-laag en gebruikt ADO.NET voor SQL Server.
De Utils-laag bevat factories om de juiste repositories en bestandslezers aan te maken.

De UI kent de DL niet rechtstreeks.

Structuur van de solution

De solution bestaat uit deze projecten:

UI
BL
DL
Utils
TestMeerKeuzevragen
Database

De applicatie gebruikt een SQL Server database.

Database naam:

Meerkeuzevragen

Belangrijkste tabellen:

Onderwerp
Vraag
Antwoord
Test
TestVraag
TestVraagAntwoord
Gebruiker
TestResultaat
GebruikerAntwoord
Applicatie opstarten
Open de solution in Visual Studio.
Maak de database Meerkeuzevragen aan in SQL Server.
Voer het SQL-script uit om de tabellen aan te maken.
Controleer de connection string in UI/appsettings.json.
Stel UI in als startup project.
Start de applicatie.
Connection string

Voorbeeld van appsettings.json:

{
  "ConnectionStrings": {
    "SqlServer": "Data Source=.;Initial Catalog=Meerkeuzevragen;Integrated Security=True;TrustServerCertificate=True"
  },
  "DatabaseType": "SqlServer"
}

Pas Data Source aan indien nodig.

Voorbeelden:

.
localhost
localhost\\SQLEXPRESS
(localdb)\\MSSQLLocalDB
Vragen importeren

De applicatie ondersteunt twee soorten bestanden.

Type 1: correct antwoord per vraag
1. Waar staat SQL voor?

A. Structured Query Language
B. Simple Query Language
C. Sequential Query Language
D. System Query Logic

Correct: A
Type 2: antwoordenlijst op het einde
1. Waar staat SQL voor?

A. Structured Query Language
B. Simple Query Language
C. Sequential Query Language
D. System Query Logic

Antwoorden
A

Bestanden met vragen moeten geïmporteerd worden via het scherm voor vragen importeren.

Test samenstellen

Bij het samenstellen van een test kiest de gebruiker:

een testnaam;
een onderwerp;
het aantal vragen.

Alle vragen binnen één test hebben hetzelfde aantal antwoordmogelijkheden.

De antwoorden worden willekeurig door elkaar gezet. De volgorde en letters van de antwoorden worden opgeslagen zodat de test correct verbeterd kan worden.

Test exporteren

Een test kan geëxporteerd worden naar een txt-bestand.

Het exportbestand bevat:

testnaam;
onderwerp;
vragen;
antwoordmogelijkheden met letters.

De juiste antwoorden worden niet getoond in het exportbestand.

Test uitvoeren

Een gebruiker kan een test uitvoeren.

Na het verbeteren toont de applicatie:

de score;
het juiste antwoord bij fout beantwoorde vragen.
Bulkresultaten verwerken

Bulkresultaten moeten dit formaat hebben:

TestId,IDGebruiker,Antwoorden
1,1,DDDDA

Betekenis:

TestId: de test die verbeterd moet worden;
IDGebruiker: de gebruiker die de test heeft afgelegd;
Antwoorden: de gekozen antwoorden als letters.

Het aantal letters moet gelijk zijn aan het aantal vragen in de test.

Bestanden met vragen, zoals SQL_Beginner.txt of c_1.txt, zijn geen bulkbestanden.

Unit tests

De unit tests staan in het project:

TestMeerKeuzevragen

De tests gebruiken xUnit en testen vooral de domeinlogica uit de BL-laag.

Voorbeelden van geteste onderdelen:

validatie van domeinklassen;
exact één juist antwoord per vraag;
toevoegen van antwoorden;
bewaren van volgorde en letters binnen een test;
berekenen van score.

De tests kunnen uitgevoerd worden via Test Explorer in Visual Studio.

Ontwerpkeuzes
Moeilijkheidsgraad werd niet geïmplementeerd omdat dit een extra uitbreiding was.
Er wordt geen aparte feedbacktekst opgeslagen; het juiste antwoord tonen is voldoende.
Er wordt niet bijgehouden uit welk bronbestand een vraag komt.
Vragen worden niet verwijderd, maar op niet beschikbaar gezet.
De volgorde van antwoorden binnen een test wordt opgeslagen voor export, uitvoering en bulkverbetering.

Als jouw `appsettings.json` andere namen gebruikt dan `SqlServer` of `DatabaseType`, pas alleen dat stukje aan.