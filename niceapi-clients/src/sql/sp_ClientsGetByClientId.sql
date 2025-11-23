DROP PROCEDURE IF EXISTS sp_ClientsGetByClientId;

CREATE DEFINER=`admin`@`%` PROCEDURE `sp_ClientsGetByClientId`(
    IN p_ClientId VARCHAR(20),
    IN p_UserId INT
)
BEGIN
    SELECT 
        ClientId, 
        Name, 
        Phone, 
        Comments, 
        UserId
    FROM 
        `Clients`
    WHERE 
        ClientId = p_ClientId AND
        UserId = p_UserId;
END