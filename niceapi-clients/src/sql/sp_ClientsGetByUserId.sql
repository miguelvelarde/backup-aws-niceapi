CREATE DEFINER=`admin`@`%` PROCEDURE `sp_ClientsGetByUserId`(
    IN p_UserId VARCHAR(20)
)
BEGIN
    SELECT ClientId, Name, Phone, Comments, UserId
    FROM `Clients`
    WHERE UserId = p_UserId;
END