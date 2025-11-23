CREATE DEFINER=`admin`@`%` PROCEDURE `sp_GetSalesByUser`(
    IN p_UserId INT,
    IN p_Records INT
)
BEGIN
    
    SELECT
        s.SaleId,
        s.SaleDate,
        s.ClientId,
        s.UserId,
        s.ProductId,
        s.Quantity,
        s.Price,
        s.Total,
        s.Status,
        s.Comments,
        s.NoTicket,
        s.SaleType        
    FROM Sales s
    WHERE s.UserId = p_UserId
    ORDER BY s.SaleDate DESC
    LIMIT p_records;

END