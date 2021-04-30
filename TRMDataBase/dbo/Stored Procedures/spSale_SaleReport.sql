﻿CREATE PROCEDURE [dbo].[spSale_SaleReport]

AS
begin

set nocount on;

	SELECT s.SaleDate,s.SubTotal,s.Tax,s.Total,u.FirstName,u.LastName,u.EmailAddress
	from dbo.sale s
	inner join dbo.[user] u on s.CashierId=u.Id
end
