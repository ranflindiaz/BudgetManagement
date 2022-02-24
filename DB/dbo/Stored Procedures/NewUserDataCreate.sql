-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[NewUserDataCreate]
	@UserId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Declare @Cash nvarchar(50) = 'Cash';
	Declare @BankAccounts nvarchar(50) = 'Bank Accounts';
	Declare @CreditCards nvarchar(50) = 'Credit Cards';
	Declare @Loan nvarchar(50) = 'Loan';

	Insert Into AccountsTypes(Name, UserId, Orden)
	Values(@Cash, @UserId, 1),
	(@BankAccounts, @UserId, 2),
	(@CreditCards, @UserId, 3),
	(@Loan, @UserId, 4);

	Insert Into Accounts(Name, Balance, AccountTypeId)
	Select Name, 0, Id
	From AccountsTypes
	Where UserId = @UserId;

	Insert Into Categories(Name, OperationTypeId, UserId)
	Values
	('Books', 2, @UserId),
	('Gas', 2, @UserId),
	('Salary', 1, @UserId),
	('Food', 2, @UserId),
	('Miscellaneous', 2, @UserId),
	('Other Income', 1, @UserId)
END
