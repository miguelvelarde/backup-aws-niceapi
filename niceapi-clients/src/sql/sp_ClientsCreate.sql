CREATE DEFINER=`admin`@`%` PROCEDURE `sp_ClientsCreate`(
    IN p_Name VARCHAR(100),
    IN p_Phone VARCHAR(10),
    IN p_Comments VARCHAR(255),
    IN p_UserId INT
)
BEGIN
    DECLARE newClientId INT;

    INSERT INTO Clients(Name, Phone, Comments, UserId)
    VALUES (p_Name, p_Phone, p_Comments, p_UserId);

    SET newClientId = LAST_INSERT_ID();

    CALL sp_InsertAuditLog(
    'Clients',
    'INSERT',
    newClientId,
    p_UserId,
    "v_OldData",
    "v_NewData");

    SELECT
        ClientId,
        Name,
        Phone,
        Comments,
        UserId
    FROM Clients
    WHERE ClientId = newClientId;
END