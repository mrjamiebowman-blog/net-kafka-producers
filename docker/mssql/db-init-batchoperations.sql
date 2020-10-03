USE master
GO
CREATE DATABASE mrjb_KafkaProducers
GO
USE [mrjb_KafkaProducers]
GO 

/* create Customers table */
CREATE TABLE [dbo].[Customers](
	[CustomerId] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](100) NOT NULL,
	[LastName] [varchar](100) NOT NULL,
	[Email] [varchar](255) NOT NULL,
	[BirthDate] [datetime] NULL,
	[BillingAddressId] [int] NULL,
	[ShippingAddressId] [int] NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Customers]  WITH CHECK ADD  CONSTRAINT [FK_Customers_BillingAddresses] FOREIGN KEY([BillingAddressId])
REFERENCES [dbo].[Addresses] ([AddressId])
GO

ALTER TABLE [dbo].[Customers] CHECK CONSTRAINT [FK_Customers_BillingAddresses]
GO

ALTER TABLE [dbo].[Customers]  WITH CHECK ADD  CONSTRAINT [FK_Customers_ShippingAddresses] FOREIGN KEY([ShippingAddressId])
REFERENCES [dbo].[Addresses] ([AddressId])
GO

ALTER TABLE [dbo].[Customers] CHECK CONSTRAINT [FK_Customers_ShippingAddresses]
GO