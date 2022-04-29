CREATE TABLE [dbo].[Waybills](
	[WaybillID] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NULL,
	[WaybillNo] [varchar](50) NULL,
	[WaybillDate] [datetime] NULL,
	[SendCustomerSiteMapID] [int] NULL,
	[ReceiveCustomerSiteMapID] [int] NULL,
	[ManifestID] [varchar](50) NULL,
	[CollectionID] [int] NULL,
	[DeliveryID] [int] NULL,
	[InvoiceID] [int] NULL,
	[RepID] [int] NULL,
	[WaybillStatusID] [int] NULL,
	[FinancialStatusID] [int] NULL,
	[LoadTypeID] [int] NULL,
	[ServiceTypeID] [int] NULL,
	[FromBranchID] [int] NULL,
	[ToBranchID] [int] NULL,
	[SubContractorDocNo] [varchar](50) NULL,
	[CustomerInvoiceValue] [decimal](18, 2) NULL,
	[TotalQty] [int] NULL,
	[TotalMass] [decimal](18, 2) NULL,
	[TotalVolume] [decimal](18, 2) NULL,
	[ChargeMass] [decimal](18, 2) NULL,
	[FreightValue] [decimal](18, 2) NULL,
	[FuelSurcharge] [decimal](18, 2) NULL,
	[DocumentationFee] [decimal](18, 2) NULL,
	[ChargesSubTotal] [decimal](18, 2) NULL,
	[Vat] [decimal](18, 2) NULL,
	[TotalValue] [decimal](18, 2) NULL,
	[LineHaulCostID] [int] NULL,
	[Insurance] [decimal](18, 2) NULL,
	[Compare1OrigValue] [decimal](18, 2) NULL,
	[Compare1NewValue] [decimal](18, 2) NULL,
	[Compare1NewChrgMass] [decimal](18, 2) NULL,
	[Compare1VariancePerc] [decimal](18, 2) NULL,
	[Compare2OrigValue] [decimal](18, 2) NULL,
	[Compare2NewValue] [decimal](18, 2) NULL,
	[Compare2NewChrgMass] [decimal](18, 2) NULL,
	[Compare2VariancePerc] [decimal](18, 2) NULL,
	[PassServiceCompliance] [bit] NULL,
	[PODDateTimeStamp] [datetime] NULL,
	[PODFWDateString] [varchar](50) NULL,
	[PODFWTimeString] [varchar](50) NULL,
	[FailServiceComplianceReason] [varchar](50) NULL,
	[FailServiceComplianceReasonCodeID] [int] NULL,
	[FailServiceComplianceReasonGroupID] [int] NULL,
	[BookingFWDateString] [varchar](50) NULL,
	[BookingFWTimeString] [varchar](50) NULL,
	[BookingDateTime] [datetime] NULL,
	[CollSubContractorID] [int] NULL,
	[DelSubContractorID] [int] NULL,
	[CustomerXRef] [varchar](50) NULL,
	[CustomerXRef2] [varchar](50) NULL,
	[ReportExclude] [bit] NOT NULL,
	[SwatExclusion] [bit] NOT NULL,
	[SwatExclusionReason] [varchar](1024) NULL,
	[FromRateAreaID] [int] NULL,
	[ToRateAreaID] [int] NULL,
	[TempRateAreaID] [int] NULL,
	[IDModify] [varchar](50) NULL,
	[IDCapture] [varchar](50) NULL,
	[FWDelExpDateString] [varchar](50) NULL,
	[FWDelExpTimeString] [varchar](50) NULL,
	[DelExpectedDateTimeStamp] [datetime] NULL,
	[DelExpectedFailed] [bit] NULL,
	[LastUpdated] [datetime] NULL,
	[FWCurrentBranch] [varchar](5) NULL,
	[CurrentBranchID] [int] NULL,
	[CollectionRequestID] [bigint] NULL,
	[CollectionManifestLineNo] [int] NULL,
	[ChainStoreDelivery] [bit] NULL,
	[PODEndorsement] [varchar](max) NULL,
	[TritonVolume] [decimal](18, 2) NULL,
	[TritonChargeMass] [decimal](18, 2) NULL,
	[DeliveryBay] [varchar](50) NULL,
	[PodRecvBy] [varchar](500) NULL,
	[FreightValueUNITS] [decimal](18, 2) NULL,
	[FuelSurchargeUNITS] [decimal](18, 2) NULL,
	[DocumentationFeeUNITS] [decimal](18, 2) NULL,
	[ChargesSubTotalUNITS] [decimal](18, 2) NULL,
	[VatUNITS] [decimal](18, 2) NULL,
	[TotalValueUNITS] [decimal](18, 2) NULL,
	[InsuranceUNITS] [decimal](18, 2) NULL,
	[RecEmail] [varchar](200) NULL,
	[TritonChainstoreFlag] [bit] NOT NULL,
 CONSTRAINT [PK_Waybills] PRIMARY KEY CLUSTERED 
(
	[WaybillID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Waybills] ADD  CONSTRAINT [DF_Waybills_ReportExclude]  DEFAULT ((0)) FOR [ReportExclude]
GO

ALTER TABLE [dbo].[Waybills] ADD  CONSTRAINT [DF_Waybills_SwatExclusion]  DEFAULT ((0)) FOR [SwatExclusion]
GO

ALTER TABLE [dbo].[Waybills] ADD  CONSTRAINT [DF_Waybills_ChainStoreDelivery]  DEFAULT ((0)) FOR [ChainStoreDelivery]
GO

ALTER TABLE [dbo].[Waybills] ADD  CONSTRAINT [DF_Waybills_TritonChainstoreFlag]  DEFAULT ((0)) FOR [TritonChainstoreFlag]
GO


