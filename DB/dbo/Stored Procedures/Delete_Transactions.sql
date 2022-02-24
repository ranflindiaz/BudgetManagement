-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE Delete_Transactions 
	@Id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Declare @Amount decimal(18,2);
	Declare @AccountId int;
	Declare @OperationTypeId int;

	Select @Amount = Amount, @AccountId = AccountId, @OperationTypeId = cat.OperationTypeId
	From Transactions
	Inner Join Categories cat
	On cat.Id = Transactions.CategoryId
	Where Transactions.Id = @Id;

	Declare @MultiplicativeFactor int = 1;

	If(@OperationTypeId = 2)
		Set @MultiplicativeFactor = -1;

	Set @Amount = @Amount * @MultiplicativeFactor;

	Update Accounts
	Set Balance -= @Amount
	Where Id = @AccountId;

	Delete Transactions
	Where Id = @Id;
END
