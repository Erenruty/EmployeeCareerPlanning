CREATE DATABASE EmployeeCareerSystem;
GO

USE EmployeeCareerSystem;
GO

CREATE TABLE Departments
(
    DepartmentID INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentName NVARCHAR(100) NOT NULL,
    ParentDepartmentID INT NULL
);
go
CREATE TABLE Positions
(
    PositionID INT IDENTITY(1,1) PRIMARY KEY,
    PositionName NVARCHAR(100) NOT NULL,
    DepartmentID INT NOT NULL,
    CareerLevel INT NOT NULL,
    MinimumExperience INT NOT NULL,
    EducationLevel NVARCHAR(50)
);
go
CREATE TABLE Employees
(
    EmployeeID INT IDENTITY(1,1) PRIMARY KEY,
    SicilNo NVARCHAR(20) UNIQUE NOT NULL,
    Ad NVARCHAR(50) NOT NULL,
    Soyad NVARCHAR(50) NOT NULL,
    Phone NVARCHAR(20),
    DepartmentID INT NOT NULL,
    PositionID INT NOT NULL,
    HireDate DATE NOT NULL,
    TotalExperienceYear INT DEFAULT 0,
    EducationLevel NVARCHAR(50),
    ForeignLanguageLevel NVARCHAR(50),
    WorkStatus NVARCHAR(30),
    ManagerID INT NULL,
    CurrentCareerLvl INT
);
go
CREATE TABLE Competencies
(
    CompetencyID INT IDENTITY(1,1) PRIMARY KEY,
    CompetencyName NVARCHAR(100) NOT NULL,
    Category NVARCHAR(100),
    Description NVARCHAR(MAX)
);
go
CREATE TABLE Employee_Competencies
(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeID INT NOT NULL,
    CompetencyID INT NOT NULL,
    CurrentLevel INT NOT NULL,
    LastUpdated DATE
);
go
CREATE TABLE Position_Requirements
(
    PositionID INT NOT NULL,
    CompetencyID INT NOT NULL,
    RequiredLevel INT NOT NULL,
    MinimumExperience INT,

    PRIMARY KEY(PositionID, CompetencyID)
);
go
CREATE TABLE Trainings
(
    TrainingID INT IDENTITY(1,1) PRIMARY KEY,
    TrainingName NVARCHAR(150) NOT NULL,
    CompetencyID INT NOT NULL,
    Level INT,
    Duration INT
);
go
CREATE TABLE Employee_Trainings
(
    EmployeeID INT NOT NULL,
    TrainingID INT NOT NULL,
    CompletionDate DATE,
    Certificate BIT DEFAULT 0,
    Score DECIMAL(5,2),
    Status NVARCHAR(30),

    PRIMARY KEY(EmployeeID, TrainingID)
);
go
CREATE TABLE Performance
(
    PerformanceID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeID INT NOT NULL,
    EvaluationDate DATE NOT NULL,
    PerformanceScore DECIMAL(5,2),
    Evaluator NVARCHAR(100)
);
go
CREATE TABLE CareerPath
(
    CareerID INT IDENTITY(1,1) PRIMARY KEY,
    CurrentPositionID INT NOT NULL,
    TargetPositionID INT NOT NULL,
    MinimumExperience INT,
    MinimumPerformance DECIMAL(5,2)
);
go
CREATE TABLE AIRecommendations
(
    RecommendationID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeID INT NOT NULL,
    TargetPositionID INT NOT NULL,
    GapScore DECIMAL(5,2),
    RecommendationText NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE()
);
go
CREATE TABLE Recommendation_Training
(
    AIRecommendationID INT NOT NULL,
    TrainingID INT NOT NULL,

    PRIMARY KEY(AIRecommendationID, TrainingID)
);
go
-- Departments
ALTER TABLE Departments
ADD CONSTRAINT FK_Department_Parent
FOREIGN KEY (ParentDepartmentID)
REFERENCES Departments(DepartmentID);

---------------------------------------------------

-- Positions

ALTER TABLE Positions
ADD CONSTRAINT FK_Position_Department
FOREIGN KEY (DepartmentID)
REFERENCES Departments(DepartmentID);

---------------------------------------------------

-- Employees

ALTER TABLE Employees
ADD CONSTRAINT FK_Employee_Department
FOREIGN KEY (DepartmentID)
REFERENCES Departments(DepartmentID);

ALTER TABLE Employees
ADD CONSTRAINT FK_Employee_Position
FOREIGN KEY (PositionID)
REFERENCES Positions(PositionID);

ALTER TABLE Employees
ADD CONSTRAINT FK_Employee_Manager
FOREIGN KEY (ManagerID)
REFERENCES Employees(EmployeeID);

---------------------------------------------------

-- Employee Competencies

ALTER TABLE Employee_Competencies
ADD CONSTRAINT FK_EmpComp_Employee
FOREIGN KEY(EmployeeID)
REFERENCES Employees(EmployeeID);

ALTER TABLE Employee_Competencies
ADD CONSTRAINT FK_EmpComp_Competency
FOREIGN KEY(CompetencyID)
REFERENCES Competencies(CompetencyID);

---------------------------------------------------

-- Position Requirements

ALTER TABLE Position_Requirements
ADD CONSTRAINT FK_PosReq_Position
FOREIGN KEY(PositionID)
REFERENCES Positions(PositionID);

ALTER TABLE Position_Requirements
ADD CONSTRAINT FK_PosReq_Competency
FOREIGN KEY(CompetencyID)
REFERENCES Competencies(CompetencyID);

---------------------------------------------------

-- Trainings

ALTER TABLE Trainings
ADD CONSTRAINT FK_Training_Competency
FOREIGN KEY(CompetencyID)
REFERENCES Competencies(CompetencyID);

---------------------------------------------------

-- Employee Trainings

ALTER TABLE Employee_Trainings
ADD CONSTRAINT FK_EmpTraining_Employee
FOREIGN KEY(EmployeeID)
REFERENCES Employees(EmployeeID);

ALTER TABLE Employee_Trainings
ADD CONSTRAINT FK_EmpTraining_Training
FOREIGN KEY(TrainingID)
REFERENCES Trainings(TrainingID);

---------------------------------------------------

-- Performance

ALTER TABLE Performance
ADD CONSTRAINT FK_Performance_Employee
FOREIGN KEY(EmployeeID)
REFERENCES Employees(EmployeeID);

---------------------------------------------------

-- Career Path

ALTER TABLE CareerPath
ADD CONSTRAINT FK_Career_CurrentPosition
FOREIGN KEY(CurrentPositionID)
REFERENCES Positions(PositionID);

ALTER TABLE CareerPath
ADD CONSTRAINT FK_Career_TargetPosition
FOREIGN KEY(TargetPositionID)
REFERENCES Positions(PositionID);

---------------------------------------------------

-- AI Recommendations

ALTER TABLE AIRecommendations
ADD CONSTRAINT FK_AIRecommendation_Employee
FOREIGN KEY(EmployeeID)
REFERENCES Employees(EmployeeID);

ALTER TABLE AIRecommendations
ADD CONSTRAINT FK_AIRecommendation_TargetPosition
FOREIGN KEY(TargetPositionID)
REFERENCES Positions(PositionID);

---------------------------------------------------

-- Recommendation Training

ALTER TABLE Recommendation_Training
ADD CONSTRAINT FK_RecTraining_Recommendation
FOREIGN KEY(AIRecommendationID)
REFERENCES AIRecommendations(RecommendationID);

ALTER TABLE Recommendation_Training
ADD CONSTRAINT FK_RecTraining_Training
FOREIGN KEY(TrainingID)
REFERENCES Trainings(TrainingID);
