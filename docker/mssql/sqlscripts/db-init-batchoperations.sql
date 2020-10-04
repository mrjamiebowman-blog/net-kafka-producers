USE master
GO
CREATE DATABASE mrjb_batchoperations
GO
USE [mrjb_batchoperations]
GO 

/* customers */
CREATE TABLE [dbo].[Customers](
	[CustomerId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/* purchase orders */
CREATE TABLE [dbo].[PurchaseOrders](
	[PurchaseOrderId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NULL,
	[PoNumber] [varchar](20) NOT NULL,
	[Amount] [decimal](18, 0) NOT NULL,
	[OnCreated] [datetime2](7) NOT NULL,
	[OnModified] [datetime2](7) NOT NULL,
	[OnProduced] [datetime2](7) NULL,
 CONSTRAINT [PK_PurchaseOrders] PRIMARY KEY CLUSTERED 
(
	[PurchaseOrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [PO_PoNumber] UNIQUE NONCLUSTERED 
(
	[PoNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PurchaseOrders]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrders_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerId])
GO

ALTER TABLE [dbo].[PurchaseOrders] CHECK CONSTRAINT [FK_PurchaseOrders_Customers]
GO



-- =============================================
-- Author:		Jamie Bowman
-- Create date: 10/03/2020
-- Description:	Create Purchase Order
-- =============================================
CREATE PROCEDURE [dbo].[uspCreatePurchaseOrder]
	@CustomerId INT,
	@PoNumber VARCHAR(20),
	@Amount DECIMAL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO PurchaseOrders 
	(
	 CustomerId
	,PoNumber
	,Amount
	,OnCreated
	,OnModified
	)
	VALUES
	(
	 @CustomerId
	,@PoNumber
	,@Amount
	,GETUTCDATE()
	,GETUTCDATE()
	)
END
GO






/* insert data */
SET IDENTITY_INSERT dbo.Customers ON
INSERT INTO dbo.Customers (CustomerId, [Name]) VALUES (1, 'Customer A');
INSERT INTO dbo.Customers (CustomerId, [Name]) VALUES (2, 'Customer B');
SET IDENTITY_INSERT dbo.Customers OFF

SELECT * FROM dbo.Customers

/* exec stored procs */
EXEC [dbo].[uspCreatePurchaseOrder] @CustomerId = 1, @PoNumber = 'ABC1234', @Amount = 2345.54
EXEC [dbo].[uspCreatePurchaseOrder] @CustomerId = 1, @PoNumber = 'ABC1235', @Amount = 4578.12
EXEC [dbo].[uspCreatePurchaseOrder] @CustomerId = 1, @PoNumber = 'ABC1236', @Amount = 9765.33

EXEC [dbo].[uspCreatePurchaseOrder] @CustomerId = 2, @PoNumber = 'ABC5679', @Amount = 1245.34
EXEC [dbo].[uspCreatePurchaseOrder] @CustomerId = 2, @PoNumber = 'ABC5610', @Amount = 7456.01
EXEC [dbo].[uspCreatePurchaseOrder] @CustomerId = 2, @PoNumber = 'ABC5611', @Amount = 3209.01

