﻿CREATE PROCEDURE [dbo].[spProduct_GetById]
	@Id int 
As
begin
    set nocount on;
	
	SELECT Id,ProductName,[Description],RetailPrice,QuantityInStock,IsTaxable
	from dbo.Product
	where Id = @Id

end
