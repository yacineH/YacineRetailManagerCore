CREATE PROCEDURE [dbo].[spSale_Insert]
	@Id int output,
	@CashierID nvarchar(128),
	@SaleDate datetime2,
	@SubTotal money,
	@Tax money,
	@Total money
AS
begin
    set nocount on;

	insert into dbo.Sale(CashierId,SaleDate,SubTotal,Tax,Total)
	values(@CashierId,@SaleDate,@SubTotal,@Tax,@Total)

	--return le id de l'objet inserer
	select @Id = Scope_Identity(); 
end
