import { LoadingButton } from "@mui/lab";
import DateTimePicker from '@mui/lab/DateTimePicker';
import { Box, Button, CircularProgress, Grid, TextField, Typography } from "@mui/material";
import { parseISO } from "date-fns";
import { useEffect, useState } from "react";
import { SubmitHandler, useFieldArray, useForm, useWatch } from "react-hook-form";
import { FormDateField } from "src/components/form-fields/form-date-field";
import { FormField } from "src/components/form-fields/form-field";
import { FormSelectField } from "src/components/form-fields/form-select-field";
import { useBranchesWithStorageLocations } from "src/hooks/use-branches-with-storage-locations";
import { useProducts } from "src/hooks/use-products";
import { useSuppliers } from "src/hooks/use-suppliers";
import products from "src/pages/products";
import { apiService } from "src/services/api";
import ProductsForm from "./test";

export interface ProcurementFormData {
    procurementTransactionId?: string;
    branchId: string;
    storageLocationId: string;
    supplierId: string;
    transactionDate: Date;
    products: ProductEntry[]
    payment?: Payment
}

interface Payment {
    payedAmount: string;
    paymentMethod: string;
}

interface ProductEntry {
    productInstanceId: string;
    quantity: string;
    unitPrice: string;
    units?: ProductUnit[]
}

interface ProductUnit {
    serialNumber: string
    expirationDate?: string
}

interface ProcurementFormProps {
    procurementTransactionId?: string;
    onSubmitSuccess: () => void;
}

export function ProcurementForm({ procurementTransactionId, onSubmitSuccess }: ProcurementFormProps) {
    const [loading, setLoading] = useState(false);
    const [submissionError, setSubmissionError] = useState<string | null>(null);
    const [fetchingProcurement, setFetchingProcurement] = useState(false);
    const [selectedProducts, setSelectedProducts] = useState<Set<string>>(new Set());
    const isEditMode = !!procurementTransactionId;

    const { control, handleSubmit, reset, setValue, setError, clearErrors, formState: { errors, dirtyFields } } = useForm<ProcurementFormData>({
        defaultValues: {
            procurementTransactionId: '',
            branchId: '',
            storageLocationId: '',
            supplierId: '',
            transactionDate: new Date,
            products: [],
            payment: { payedAmount: '', paymentMethod: '' }
        },
    });

    const { fields, append, remove } = useFieldArray({
        control,
        name: 'products',
    });

    const watchFieldArray = useWatch({ control, name: "products" });

    const controlledFields = fields.map((field, index) => ({
        ...field,
        ...watchFieldArray[index],
    }));

    const watchBranchId = useWatch({ control, name: "branchId" });

    const handleAddProduct = () => {
        append({ productInstanceId: '', quantity: '', unitPrice: '', units: [] });
    };

    const handleProductChange = (index: number, productInstanceId: string) => {
        console.log(index, productInstanceId, selectedProducts)
        const product = productsResponse?.value?.find((p) => p.productInstanceId === productInstanceId);
        if (product) {
            if (selectedProducts.has(productInstanceId)) {
                setError(`products.${index}.productInstanceId`, {
                    type: 'manual',
                    message: 'This product has already been selected.',
                });
            } else {
                clearErrors(`products.${index}.productInstanceId`);
                setSelectedProducts((prev) => new Set([...prev, productInstanceId]));
                console.log(selectedProducts)
            }
        }
    };

    const handleQuantityChange = (index: number, quantity: number) => {
        const productInstanceId = fields[index]?.productInstanceId;
        const product = productsResponse?.value.find((p) => p.productInstanceId === productInstanceId);
        if (product && (product.isWarranted || product.shelfLifeInDays)) {
            const serialNumbers = Array(quantity).fill('');
            console.log(serialNumbers)
            setValue(`products.${index}.units`, serialNumbers);
        }
    };

    useEffect(() => {
        setValue('storageLocationId', '');
    }, [watchBranchId, setValue]);

    useEffect(() => {
        const fetchProcurementData = async () => {
            if (procurementTransactionId) {
                setFetchingProcurement(true);
                const response = await apiService.procurements.getById(Number(procurementTransactionId))
                if (!response.isSuccess) {
                    setSubmissionError('Failed to load procurement transaction data. Please try again.');
                }
                else {
                    reset(response.value)
                }
                setFetchingProcurement(false);
            }
        };

        fetchProcurementData();
    }, [procurementTransactionId, reset]);

    const onSubmit: SubmitHandler<ProcurementFormData> = async (data) => {
        setLoading(true);
        setSubmissionError(null);
        try {
            if (isEditMode) {
                const changedData = Object.fromEntries(
                    Object.entries(dirtyFields)
                        .filter(([_, isDirty]) => isDirty)
                        .map(([key]) => [key, data[key as keyof ProcurementFormData]])
                ) as Partial<ProcurementFormData>;
                const requestBody = {
                    ...changedData,
                    procurementTransactionId: procurementTransactionId
                };
                await apiService.procurements.update(procurementTransactionId, requestBody);
            } else {
                await apiService.procurements.create(data);
            }
            console.log(isEditMode ? 'Procurement transaction updated successfully' : 'Procurement transaction added successfully');
            onSubmitSuccess();
        } catch (error: any) {
            console.error(error);
            setSubmissionError(error.message || "An unexpected error occurred. Please try again.");
        } finally {
            setLoading(false);
        }
    };

    const GetAssociatedStorageLocations = (branchId?: string) => {
        if (!branchId) return []
        const branch = branchesResponse?.value.find(branch => branch.branchId.toString() === branchId.toString());
        return branch ? branch.storageLocations.map(val => ({ value: val.storageLocationId, label: val.name })) : [];
    };

    const { data: productsResponse, error: productsError, isLoading: isLoadingProducts } = useProducts();
    const { data: suppliersResponse, error: suppliersError, isLoading: isLoadingSuppliers } = useSuppliers();
    const { data: branchesResponse, error: branchesError, isLoading: isLoadingBranches } = useBranchesWithStorageLocations();

    if (isLoadingProducts || isLoadingSuppliers || isLoadingBranches || fetchingProcurement) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" height="100vh">
                <CircularProgress />
                <Typography variant="h6" sx={{ marginLeft: 2 }}>
                    Loading...
                </Typography>
            </Box>
        );
    }

    if (productsError || suppliersError || branchesError) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" height="100vh">
                <Typography color="error">{productsError?.message} {suppliersError?.message} {branchesError?.message}</Typography>
            </Box>
        );
    }

    return (
        <Box component="form" autoComplete="on" onSubmit={handleSubmit(onSubmit)}>

            <Grid sx={{ display: 'flex', gap: 2, mb: 2, alignItems: 'center', flexWrap: 'wrap' }}>

                <FormSelectField<ProcurementFormData>
                    name="branchId"
                    control={control}
                    label="Branch"
                    rules={{ required: 'Branch is required' }}
                    options={branchesResponse?.value.map(branch => ({ value: branch.branchId, label: branch.name })) ?? []}
                    error={!!errors.branchId}
                    helperText={errors.branchId?.message}
                />

                <FormSelectField<ProcurementFormData>
                    name="storageLocationId"
                    control={control}
                    label="Storage Location"
                    rules={{ required: 'Storage Location is required' }}
                    options={GetAssociatedStorageLocations(watchBranchId)}
                    error={!!errors.storageLocationId}
                    helperText={errors.storageLocationId?.message}
                />

            </Grid>

            <Grid sx={{ display: 'flex', gap: 2, mb: 2, alignItems: 'center', flexWrap: 'wrap' }}>

                <FormSelectField<ProcurementFormData>
                    name="supplierId"
                    control={control}
                    label="Supplier"
                    rules={{ required: 'Supplier is required' }}
                    options={suppliersResponse?.value ?? []}
                    error={!!errors.supplierId}
                    helperText={errors.supplierId?.message}
                />

                <FormDateField<ProcurementFormData>
                    name="transactionDate"
                    control={control}
                    label="Transaction Date"
                />

            </Grid>

            <Typography variant="subtitle1" gutterBottom sx={{ mt: 2 }}>
                Payment
            </Typography>

            <Grid sx={{ display: 'flex', gap: 2, mb: 2, alignItems: 'center', flexWrap: 'wrap' }}>

                <FormField<ProcurementFormData>
                    name="payment.payedAmount"
                    control={control}
                    label="Payed Amount"
                    isNumber={true}
                    error={!!errors.payment?.payedAmount}
                    helperText={errors.payment?.payedAmount?.message}
                />

                <FormField<ProcurementFormData>
                    name="payment.paymentMethod"
                    control={control}
                    label="Payed Method"
                    error={!!errors.payment?.paymentMethod}
                    helperText={errors.payment?.paymentMethod?.message}
                />

            </Grid>

            <Typography variant="subtitle1" gutterBottom sx={{ mt: 2 }}>
                Products
            </Typography>

            <ProductsForm
                control={control}
                setValue={setValue}
                errors={errors}
                productOptions={productsResponse?.value.map((product) => ({
                    value: product.productInstanceId,
                    label: product.name,
                    isTracked: (product.isWarranted || product.shelfLifeInDays != null),
                    expirationDate: product.shelfLifeInDays != null
                        ? new Date(Date.now() + product.shelfLifeInDays * 24 * 60 * 60 * 1000)
                        : undefined
                })) ?? []}
            />

            <LoadingButton
                fullWidth
                size="large"
                type="submit"
                variant="contained"
                sx={{ mt: 3 }}
                loading={loading}
            >
                {isEditMode ? 'Update Procurement' : 'Add Procurement'}
            </LoadingButton>

            {submissionError && (
                <Typography color="error" sx={{ mt: 2 }}>
                    {submissionError}
                </Typography>
            )}

            {loading && (
                <Box
                    sx={{
                        position: 'fixed',
                        top: 0,
                        left: 0,
                        right: 0,
                        bottom: 0,
                        backgroundColor: 'rgba(0, 0, 0, 0.5)',
                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'center',
                        zIndex: 1000,
                    }}
                >
                    <CircularProgress />
                </Box>
            )}
        </Box>
    )
}