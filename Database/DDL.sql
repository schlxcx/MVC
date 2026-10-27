DROP DATABASE IF EXISTS mychatness;

CREATE DATABASE mychatness;

USE mychatness;

CREATE TABLE Country (country_id INT AUTO_INCREMENT PRIMARY KEY, country VARCHAR(50) NOT NULL);

CREATE TABLE City (city_id INT AUTO_INCREMENT PRIMARY KEY, city VARCHAR(50) NOT NULL);

CREATE TABLE Gender (gender_id INT AUTO_INCREMENT PRIMARY KEY, gender VARCHAR(20) NOT NULL);

CREATE TABLE Permission (permission_id INT AUTO_INCREMENT PRIMARY KEY, permission VARCHAR(20));

CREATE TABLE Setting (
	setting_id INT AUTO_INCREMENT PRIMARY KEY,
    show_real_name BOOLEAN NOT NULL,
    show_email BOOLEAN NOT NULL,
    show_gender BOOLEAN NOT NULL,
    show_working_section BOOLEAN NOT NULL,
    show_country BOOLEAN NOT NULL,
    show_city BOOLEAN NOT NULL,
    show_activity_status BOOLEAN NOT NULL,
    show_read_receiption BOOLEAN NOT NULL);

CREATE TABLE WorkingSection (workingSection_id INT AUTO_INCREMENT PRIMARY KEY, workingSection VARCHAR(50));

CREATE TABLE User (
	user_id INT AUTO_INCREMENT NOT NULL,
	username VARCHAR(30) NOT NULL,
    email VARCHAR(255) NOT NULL,
    lastname VARCHAR(20),
    firstname VARCHAR(20),
    password VARCHAR(255),
    p_picture_path VARCHAR(255),
    b_picture_path VARCHAR(255),
    color VARCHAR(20),
    secondary_color VARCHAR(20),
    bio TEXT,
    country_id INT,
    city_id INT,
    gender_id INT,
    permission_id INT,
    setting_id INT,
    FOREIGN KEY (country_id)
		REFERENCES Country(country_id),
	FOREIGN KEY (city_id)
		REFERENCES City(city_id)
        ON DELETE SET NULL,
	FOREIGN KEY (gender_id)
		REFERENCES Gender(gender_id),
	FOREIGN KEY (setting_id)
		REFERENCES Setting(setting_id)
        ON DELETE CASCADE,
	FOREIGN KEY (permission_id)
		REFERENCES Permission(permission_id)
        ON DELETE SET NULL,
	PRIMARY KEY (user_id, username));
    
CREATE TABLE Tokentype (
	tokentype_id INT AUTO_INCREMENT PRIMARY KEY,
    tokentype VARCHAR(50));
    
CREATE TABLE Token (
	token_id INT AUTO_INCREMENT PRIMARY KEY,
    token VARCHAR(255),
    expires_at DATETIME NOT NULL,
    created_at DATETIME NOT NULL,
    tokentype_id INT NOT NULL,
    user_id INT NOT NULL,
    FOREIGN KEY (tokentype_id)
		REFERENCES Tokentype(tokentype_id),
	FOREIGN KEY (user_id)
		REFERENCES User(user_id));
    
    
CREATE TABLE User_WorkingSection (
    user_workingSection_id INT AUTO_INCREMENT PRIMARY KEY,
    workingSection_id INT NOT NULL,
    user_id INT NOT NULL,
    FOREIGN KEY (workingSection_id)
        REFERENCES WorkingSection(workingSection_id),
    FOREIGN KEY (user_id)
        REFERENCES User(user_id)
);
    
CREATE TABLE Organisation (
	organisation_id INT AUTO_INCREMENT PRIMARY KEY,
    organisationName VARCHAR(30) NOT NULL,
    b_picture_path VARCHAR(255),
    color VARCHAR(20),
    bio TEXT);
    
CREATE TABLE User_Organisation (
	user_organisation INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    organisation_id INT NOT NULL,
    permission_id INT,
    FOREIGN KEY (user_id)
		REFERENCES User(user_id)
        ON DELETE CASCADE,
	FOREIGN KEY (organisation_id)
		REFERENCES Organisation(organisation_id)
        ON DELETE CASCADE,
	FOREIGN KEY (permission_id)
		REFERENCES Permission(permission_id)
        ON DELETE SET NULL);
	
CREATE TABLE Organisation_WorkingSection (
    organisation_workingSection_id INT AUTO_INCREMENT PRIMARY KEY,
    workingSection_id INT NOT NULL,
    organisation_id INT NOT NULL,
    FOREIGN KEY (workingSection_id)
        REFERENCES WorkingSection(workingSection_id),
    FOREIGN KEY (organisation_id)
        REFERENCES Organisation(organisation_id)
);

CREATE TABLE Section (section_id INT AUTO_INCREMENT PRIMARY KEY, organisation_id INT NOT NULL, FOREIGN KEY (organisation_id) REFERENCES Organisation(organisation_id));

CREATE TABLE User_Section (
	user_section INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    section_id INT NOT NULL,
    permission_id INT,
    FOREIGN KEY (user_id)
		REFERENCES User(user_id)
        ON DELETE CASCADE,
	FOREIGN KEY (section_id)
		REFERENCES Section(section_id)
        ON DELETE CASCADE,
	FOREIGN KEY (permission_id)
		REFERENCES Permission(permission_id)
        ON DELETE SET NULL);
        
CREATE TABLE Contact (
	contact_id INT AUTO_INCREMENT PRIMARY KEY,
    first_user_id INT NOT NULL,
    has_first_user_seen BOOLEAN,
    second_user_id INT NOT NULL,
    has_second_user_seen BOOLEAN,
    FOREIGN KEY (first_user_id)
		REFERENCES User(user_id),
	FOREIGN KEY (second_user_id)
		REFERENCES User(user_id));
        
CREATE TABLE Message (
	message_id INT AUTO_INCREMENT PRIMARY KEY,
    message TEXT NOT NULL,
    sent_at DATETIME NOT NULL,
    isContact BOOLEAN NOT NULL,
    section_id INT,
    contact_id INT,
    FOREIGN KEY (section_id)
		REFERENCES Section(section_id)
        ON DELETE CASCADE,
	FOREIGN KEY (contact_id)
		REFERENCES Contact(contact_id)
        ON DELETE CASCADE);
    
