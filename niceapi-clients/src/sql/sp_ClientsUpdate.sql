DROP PROCEDURE IF EXISTS `sp_ClientsUpdate`;

CREATE DEFINER=`admin`@`%` PROCEDURE `sp_ClientsUpdate`(
    IN p_Name VARCHAR(100),
    IN p_Phone VARCHAR(10),
    IN p_Comments VARCHAR(255),
    IN p_UserId INT,
    IN p_ClientId INT
)
BEGIN

    UPDATE Clients SET
    Name = p_Name,
    Phone = p_Phone,
    Comments = p_Comments
    WHERE
    ClientId = p_ClientId AND
    UserId = p_UserId;

    CALL sp_InsertAuditLog(
    'Clients',
    'UPDATE',
    p_ClientId,
    p_UserId,
    "v_OldData",
    "v_NewData");

    SELECT
        c.ClientId,
        c.Name,
        c.Phone,
        c.Comments,
        c.UserId
    FROM Clients c
    WHERE 
        c.ClientId = p_ClientId AND
        UserId = p_UserId;
END