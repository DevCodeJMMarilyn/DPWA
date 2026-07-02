USE dbOnionAPI
GO

SELECT Id, Nombre, Email, Rol, Activo
FROM Usuarios;

USE dbOnionAPI;
GO

INSERT INTO Usuarios
(
    Nombre,
    Email,
    PasswordHash,
    Rol,
    Activo,
    Departamento,
    Distrito,
    DireccionCercana,
    FechaRegistro
)
VALUES
(
    'Administrador Master',
    'master@sistema.com',
    '$2a$11$QKRkBgyVBwHAeKcrGKBl/.Vqk1QQn/Fvb7duodmneLGk511wDjyy2',
    1,
    1,
    'Todo el país',
    'Todo el país',
    'Administrador general',
    GETDATE()
);
SELECT *
FROM Usuarios
WHERE Email = 'master@sistema.com';
DELETE FROM Usuarios
WHERE Email = 'master@sistema.com';