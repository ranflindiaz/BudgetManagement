-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE Insert_Transaction 
	-- Add the parameters for the stored procedure here
	@UserId int,
	@TransactionDate date,
	@Amount decimal(18,2),
	@CategoryId int,
	@AccountId int,
	@Note nvarchar(1000) = Null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Insert Into Transactions (UserId, TransactionDate, Amount, CategoryId, AccountId, Note)
	Values(@UserId, @TransactionDate, ABS(@Amount), @CategoryId, @AccountId, @Note)

	Update Accounts
	Set Balance += @Amount
	Where Id = @AccountId;

	Select SCOPE_IDENTITY();
END
