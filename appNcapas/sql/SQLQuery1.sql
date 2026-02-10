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

CREATE TABLE habitaciones (
    id_habitacion INT IDENTITY(1,1) PRIMARY KEY,
    numero INT NOT NULL UNIQUE, --arreglar tabla para que no quede unico
    descripcion VARCHAR(150),
    cant_huespedes INT NOT NULL
);
INSERT INTO habitaciones (numero, descripcion, cant_huespedes)
VALUES
(1, 'Habitación sencilla con cama individual', 1),
(2, 'Habitación doble con cama matrimonial', 2),
(3, 'Habitación doble con dos camas', 2);

Select * from habitaciones