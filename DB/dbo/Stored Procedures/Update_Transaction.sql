-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE Update_Transaction 
	@Id int,
	@TransactionDate datetime,
	@Amount decimal(18,2),
	@PreviousAmount decimal(18,2),
	@AccountId int,
	@PreviousAccountId int,
	@CategoryId int,
	@Note nvarchar(1000) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--Reverse previous transaction
    Update Accounts
	Set Balance -= @PreviousAmount
	where Id = @PreviousAccountId

	--Do new transaction
	Update Accounts
	Set Balance += @Amount
	Where Id = @AccountId

	Update Transactions
	Set Amount = ABS(@Amount), TransactionDate = @TransactionDate,
	CategoryId = @CategoryId, AccountId = @AccountId, Note = @Note
	Where Id = @Id;

END
