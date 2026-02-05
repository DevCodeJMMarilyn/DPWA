CREATE DATABASE LoginDB;
GO

USE LoginDB;
GO

CREATE TABLE Usuario (
    Usuario VARCHAR(50) NOT NULL,
    Clave   VARCHAR(100) NOT NULL
);
GO

INSERT INTO Usuario (Usuario, Clave)
VALUES ('admin', '1234');
GO

DECLARE @u VARCHAR(50) = 'admin';
DECLARE @c VARCHAR(100) = '1234';

SELECT COUNT(*) FROM Usuario WHERE Usuario=@u AND Clave=@c