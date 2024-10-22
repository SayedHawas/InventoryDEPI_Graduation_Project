import { Box, Typography, Button, Card, CircularProgress, Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from "@mui/material";
import { useCallback, useState } from "react";
import { Iconify } from "src/components/iconify";
import { DashboardContent } from "src/layouts/dashboard";
import { ProcurementForm } from "../procurement-form";
import { CustomDialog } from "src/layouts/components/custom-dialog";
import { GenericTable, TableAction } from "src/layouts/components/table/generic-table";
import { TableColumn } from "src/services/types";
import { useEntities } from "src/hooks/use-entities";
import { useTable } from "src/hooks/use-table";
import { GenericTableRow } from "src/layouts/components/table/generic-table-row";
import { PaymentForm } from "../payment-form";
import { ProductForm } from "../product-form";
import { FormDateField } from "src/components/form-fields/form-date-field";

type ProcurementProps = {
    id: string;
    supplier: string;
    branch: string;
    storageLocation: string;
    leftAmount: number;
    transactionDate: string;
    payments: PaymentProps[]
    products: ProductProps[]
}

type ProductProps = { id: string; name: string; quantity: number; unitPrice: number; units?: { serialNumber: string }[] }

type PaymentProps = { id: string; payedAmount: string; payedMethod: string }

const transformPayment = (apiPayment: any): PaymentProps => {
    return {
        id: apiPayment.paymentTransactionId,
        payedAmount: apiPayment.payedAmount,
        payedMethod: apiPayment.paymentMethod,
    };
};

const transformProduct = (apiProduct: any): ProductProps => {
    return {
        id: apiProduct.productInstanceId,
        name: apiProduct.name,
        quantity: apiProduct.quantity,
        unitPrice: apiProduct.unitPrice,
        units: apiProduct.units.map((unit: any) => ({ serialNumber: unit.serialNumber })),
    };
};

const transformProcurement = (apiResponse: any): ProcurementProps => {
    return {
        id: apiResponse.transactionId,
        supplier: apiResponse.supplier,
        branch: apiResponse.branch,
        storageLocation: apiResponse.storageLocation,
        leftAmount: apiResponse.leftAmount,
        transactionDate: new Date(apiResponse.transactionDate).toDateString(),
        payments: apiResponse.payments.map(transformPayment),
        products: apiResponse.products.map(transformProduct),
    };
};

const PROCUREMENT_TABLE_COLUMNS: TableColumn<ProcurementProps>[] = [
    { id: 'supplier', label: 'Supplier' },
    { id: 'branch', label: 'Branch' },
    { id: 'storageLocation', label: 'Storage Location' },
    { id: 'leftAmount', label: 'Left Amount' },
    { id: 'transactionDate', label: 'Transaction Date' },
    { id: 'warrantyInDays', label: 'Warranty' }
];

export function ProcurementView() {
    const [filterName, setFilterName] = useState('');
    const [showProcurementForm, setShowProcurementForm] = useState(false);
    const [showBaseDetailsForm, setShowBaseDetailsForm] = useState(false);
    const [showProductForm, setShowProductForm] = useState(false);
    const [showPaymentForm, setShowPaymentForm] = useState(false);
    const [selectedProcurement, setSelectedProcurement] = useState<ProcurementProps | null>(null);
    const [selectedProduct, setSelectedProduct] = useState<ProductProps | null>(null);
    const [selectedPayment, setSelectedPayment] = useState<PaymentProps | null>(null);
    const [filterDateStart, setFilterDateStart] = useState<Date | null>(null)
    const [filterDateEnd, setFilterDateEnd] = useState<Date | null>(null)

    const table = useTable();

    const { entities: procurements, loading, error, totalCount, updateParams, refetch } = useEntities<ProcurementProps>(
        'ProcurementTransactions',
        {
            PageNumber: table.page + 1,
            PageSize: table.rowsPerPage,
            SortBy: table.orderBy,
            SortDescending: table.order === 'desc',
            SearchTerm: filterName,
            FilterBy: [{ filterBy: "startDate", value: filterDateStart?.toDateString() ?? '' }, { filterBy: "endDate", value: filterDateEnd?.toDateString() ?? '' }]
        },
        transformProcurement
    );

    const handleFilterName = (event: React.ChangeEvent<HTMLInputElement>) => {
        const newFilterName = event.target.value;
        setFilterName(newFilterName);
        updateParams({ SearchTerm: newFilterName, PageNumber: 1 });
        table.onChangePage(null, 0);
    };

    const handleStartDateChange = (date: Date | null) => {
        setFilterDateStart(date);
        updateParams({
          FilterBy: [
            { filterBy: "startDate", value: date?.toDateString() ?? '' },
            { filterBy: "endDate", value: filterDateEnd?.toDateString() ?? '' }
          ],
          PageNumber: 1
        });
        table.onChangePage(null, 0);
      };
    
      const handleEndDateChange = (date: Date | null) => {
        setFilterDateEnd(date);
        updateParams({
          FilterBy: [
            { filterBy: "endDate", value: date?.toDateString() ?? '' },
            { filterBy: "startDate", value: filterDateStart?.toDateString() ?? '' },
            { filterBy: "endDate", value: filterDateEnd?.toDateString() ?? '' }
          ],
          PageNumber: 1
        });
        table.onChangePage(null, 0);
      };

    const handleChangePage = (event: unknown, newPage: number) => {
        updateParams({ PageNumber: newPage + 1 });
        table.onChangePage(event, newPage);
    };

    const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
        const newRowsPerPage = parseInt(event.target.value, 10);
        updateParams({ PageSize: newRowsPerPage, PageNumber: 1 });
        table.onChangeRowsPerPage(event);
    };

    const handleSort = (property: keyof ProcurementProps) => {
        const isAsc = table.orderBy === property && table.order === 'asc';
        updateParams({
            SortBy: property,
            SortDescending: !isAsc,
        });
        table.onSort(property);
    };

    const handleSelectRow = (event: React.ChangeEvent<HTMLInputElement>, checked: boolean, id: string) => {
        table.onSelectRow(event, checked, id);
    };


    const handleAddProcurement = () => {
        setSelectedProcurement(null);
        setShowProcurementForm(true);
    };

    const handleEditProcurement = (procurement: ProcurementProps) => {
        setSelectedProcurement(procurement);
        setShowBaseDetailsForm(true);
    };

    const handleProcurementFormClose = useCallback(() => {
        setShowProcurementForm(false);
        refetch();
    }, [refetch]);

    const handleProcurementFormCancel = () => {
        setShowProcurementForm(false);
    }

    const handleBaseDetailsFormClose = useCallback(() => {
        setShowBaseDetailsForm(false);
        refetch();
    }, [refetch]);

    const handleBaseDetailsFormCancel = () => {
        setShowBaseDetailsForm(false);
    }

    const handleAddProduct = (procurement: ProcurementProps) => {
        setSelectedProcurement(procurement);
        setSelectedProduct(null)
        setShowProductForm(true);
    };

    const handleEditProduct = (procurement: ProcurementProps, product: ProductProps) => {
        setSelectedProcurement(procurement);
        setSelectedProduct(product)
        setShowProductForm(true);
    };

    const handleProductFormClose = useCallback(() => {
        setShowProductForm(false);
        refetch();
    }, [refetch]);

    const handleProductFormCancel = () => {
        setShowProductForm(false);
    }

    const handleAddPayment = (procurement: ProcurementProps) => {
        setSelectedProcurement(procurement);
        setSelectedPayment(null)
        setShowPaymentForm(true);
    };

    const handleEditPayment = (procurement: ProcurementProps, payment: PaymentProps) => {
        console.log(payment)
        setSelectedProcurement(procurement);
        setSelectedPayment(payment)
        setShowPaymentForm(true);
    };

    const handlePaymentFormClose = useCallback(() => {
        setShowPaymentForm(false);
        refetch();
    }, [refetch]);

    const handlePaymentFormCancel = () => {
        setShowPaymentForm(false);
    }

    const tableActions: TableAction<ProcurementProps>[] = [
        {
            label: 'Add Payment',
            icon: 'mingcute:add-fill',
            onClick: (row) => handleAddPayment(row),
        },
        {
            label: 'Add Product',
            icon: 'mingcute:add-fill',
            onClick: (row) => handleAddProduct(row),
        },
        {
            label: 'Edit Base Details',
            icon: 'solar:pen-bold',
            onClick: (row) => handleEditProcurement(row),
        }
    ];

    const paymentTableActions: TableAction<PaymentProps>[] = [
        {
            label: 'Edit Payment',
            icon: 'solar:pen-bold',
            onClick: (row, procurement) => handleEditPayment(procurement, row),
        }
    ]

    const productTableActions: TableAction<ProductProps>[] = [
        {
            label: 'Edit Product',
            icon: 'solar:pen-bold',
            onClick: (row, procurement) => handleEditProduct(procurement, row),
        }
    ]

    return (
        <DashboardContent>
            <Box display="flex" alignItems="center" mb={5}>
                <Typography variant="h4" flexGrow={1}>
                    Procurement Transactions
                </Typography>
                <Button variant="contained" color="inherit" onClick={handleAddProcurement} startIcon={<Iconify icon="mingcute:add-line" />}>
                    New Transaction
                </Button>
            </Box>

            <Card>
                <TableContainer sx={{ position: 'relative', overflow: 'unset' }}>
                    <GenericTable
                        data={procurements}
                        columns={PROCUREMENT_TABLE_COLUMNS}
                        totalCount={totalCount}
                        page={table.page}
                        rowsPerPage={table.rowsPerPage}
                        orderBy={table.orderBy}
                        order={table.order}
                        selected={table.selected}
                        filterName={filterName}
                        onFilterName={handleFilterName}
                        onChangePage={handleChangePage}
                        onChangeRowsPerPage={handleChangeRowsPerPage}
                        onSort={handleSort}
                        onSelectAllRows={(checked) => table.onSelectAllRows(checked, procurements.map(procurement => procurement.id))}
                        onSelectRow={handleSelectRow}
                        getRowId={(row: ProcurementProps) => row.id}
                        actions={tableActions}
                        customFilters={
                            <>
                                <FormDateField
                                    label="Start Date"
                                    onChange={(date) => handleStartDateChange(date)}
                                />

                                <FormDateField
                                    label="End Date"
                                    onChange={(date) => handleEndDateChange(date)}
                                />
                            </>
                        }
                        expandableContent={(procurement: ProcurementProps) => (
                            <>

                                {procurement.payments.length > 0 && (
                                    <div>
                                        <Typography variant="h6" gutterBottom component="div">
                                            Payment Details
                                        </Typography>
                                        <Table size="small" aria-label="payment details">
                                            <TableHead>
                                                <TableRow>
                                                    <TableCell>Payment Method</TableCell>
                                                    <TableCell>Amount Paid</TableCell>
                                                    <TableCell />
                                                </TableRow>
                                            </TableHead>
                                            <TableBody>
                                                {procurement.payments.map((payment) => (
                                                    <GenericTableRow<PaymentProps, ProcurementProps>
                                                        key={payment.id}
                                                        row={payment}
                                                        columns={[
                                                            { id: 'payedMethod', label: 'Payment Method', align: 'left', render: () => payment.payedMethod },
                                                            { id: 'payedAmount', label: 'Amount Paid', align: 'left', render: () => payment.payedAmount },
                                                        ]}
                                                        selected={false}
                                                        getRowId={(row) => row.id}
                                                        actions={paymentTableActions}
                                                        actionContext={procurement}
                                                    />
                                                ))}
                                            </TableBody>
                                        </Table>
                                    </div>
                                )}

                                {procurement.products.length > 0 && (
                                    <div>
                                        <Typography variant="h6" gutterBottom component="div">
                                            Product Instances
                                        </Typography>
                                        <Table size="small" aria-label="product instances">
                                            <TableHead>
                                                <TableRow>
                                                    <TableCell>Product Name</TableCell>
                                                    <TableCell>Quantity</TableCell>
                                                    <TableCell>Unit Price</TableCell>
                                                    <TableCell>Units</TableCell>
                                                    <TableCell />
                                                </TableRow>
                                            </TableHead>
                                            <TableBody>
                                                {procurement.products.map((product) => (
                                                    <GenericTableRow<ProductProps, ProcurementProps>
                                                        key={product.id}
                                                        row={product}
                                                        columns={[
                                                            { id: 'name', label: 'Product Name', render: () => product.name },
                                                            { id: 'quantity', label: 'Quantity', render: () => product.quantity },
                                                            { id: 'unitPrice', label: 'Unit Price', render: () => product.unitPrice },
                                                            { id: 'units', label: 'Units', render: () => product.units ? product.units.map(unit => unit.serialNumber).join(', ') : 'N/A' }
                                                        ]}
                                                        selected={false}
                                                        getRowId={(row) => row.id}
                                                        actions={productTableActions}
                                                        actionContext={procurement}
                                                    />
                                                ))}
                                            </TableBody>
                                        </Table>
                                    </div>
                                )}
                            </>
                        )}
                    />

                    {loading && (
                        <Box
                            sx={{
                                position: 'absolute',
                                top: 0,
                                left: 0,
                                right: 0,
                                bottom: 0,
                                bgcolor: 'rgba(255, 255, 255, 0.8)',
                                display: 'flex',
                                justifyContent: 'center',
                                alignItems: 'center',
                                zIndex: 10,
                            }}
                        >
                            <CircularProgress />
                        </Box>
                    )}

                    {error && (
                        <Box
                            sx={{
                                position: 'absolute',
                                top: 0,
                                left: 0,
                                right: 0,
                                bottom: 0,
                                bgcolor: 'rgba(255, 255, 255, 0.8)',
                                display: 'flex',
                                justifyContent: 'center',
                                alignItems: 'center',
                                zIndex: 10,
                            }}
                        >
                            {error}
                        </Box>
                    )}
                </TableContainer>
            </Card>

            <CustomDialog open={showProcurementForm} handleCancel={handleProcurementFormCancel} title={selectedProcurement?.id ? 'Edit transaction' : 'Add new transaction'} content={<ProcurementForm procurementTransactionId={selectedProcurement?.id} onSubmitSuccess={handleProcurementFormClose} />} />
            <CustomDialog open={showPaymentForm} handleCancel={handlePaymentFormCancel} title={selectedPayment?.id ? 'Edit payment' : 'Add new payment'} content={<PaymentForm transactionId={selectedProcurement?.id ?? ''} paymentId={selectedPayment?.id} onSubmitSuccess={handlePaymentFormClose} />} />
            <CustomDialog open={showProductForm} handleCancel={handleProductFormCancel} title={selectedProduct?.id ? 'Edit product' : 'Add new product'} content={<ProductForm transactionId={selectedProcurement?.id ?? ''} productInstanceId={selectedProduct?.id} onSubmitSuccess={handleProductFormClose} />} />
        </DashboardContent>
    )
}