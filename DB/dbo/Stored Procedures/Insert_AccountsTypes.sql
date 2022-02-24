-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE Insert_AccountsTypes
@Name nvarchar(50),
@UserId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Declare @Orden int;
	Select @Orden = Coalesce(Max(Orden), 0)+1
	From AccountsTypes
	Where UserId = @UserId

	Insert into AccountsTypes(Name, UserId, Orden)
	Values (@Name, @UserId, @Orden)

	Select SCOPE_IDENTITY();

END
