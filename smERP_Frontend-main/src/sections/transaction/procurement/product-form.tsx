import { LoadingButton } from "@mui/lab";
import { Box, CircularProgress, FormHelperText, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { SubmitHandler, useForm, useWatch } from "react-hook-form";
import { FormDateField } from "src/components/form-fields/form-date-field";
import { FormField } from "src/components/form-fields/form-field";
import { FormSelectField } from "src/components/form-fields/form-select-field";
import { useProducts } from "src/hooks/use-products";
import { apiService } from "src/services/api";

export interface TransactionProductFormData {
    transactionId: string;
    productInstanceId?: string;
    quantity: string;
    isTracked: boolean;
    shelfLifeInDays?: number;
    unitPrice: string;
    units?: ProductUnit[];
    unitsToAdd?: ProductUnit[];
    unitsToRemove?: string[];
}

interface ProductUnit {
    serialNumber: string;
    expirationDate?: Date;
}

interface ProductFormProps {
    transactionId: string;
    productInstanceId?: string
    onSubmitSuccess: () => void;
}

export function ProductForm({ transactionId, productInstanceId, onSubmitSuccess }: ProductFormProps) {
    const [loading, setLoading] = useState(false);
    const [submissionError, setSubmissionError] = useState<string | null>(null);
    const [fetchingProduct, setFetchingProduct] = useState(false);
    const [productQuantity, setProductQuantity] = useState(0);
    const [expirationDate, setExpirationDate] = useState<Date | null>(null);
    const [originalUnits, setOriginalUnits] = useState<ProductUnit[]>([]);
    const [originalQuantity, setOriginalQuantity] = useState<number>(0);
    const isEditMode = !!productInstanceId;

    const { control, handleSubmit, reset, setValue, getValues, formState: { errors, dirtyFields } } = useForm<TransactionProductFormData>({
        defaultValues: {
            transactionId: transactionId,
            productInstanceId: productInstanceId ?? '',
            quantity: '',
            unitPrice: '',
            units: [],
            unitsToAdd: [],
            unitsToRemove: []
        },
    });

    const transformApiResponse = (apiResponse: any): TransactionProductFormData => {
        const value = apiResponse;

        return {
            transactionId: transactionId,
            productInstanceId: value.productInstanceId,
            quantity: value.quantity,
            isTracked: value.isTracked,
            shelfLifeInDays: value.shelfLifeInDays,
            unitPrice: value.unitPrice,
            units: value.units ? value.units.map((serialNumber: string) => ({ serialNumber })) : [],
            unitsToAdd: [],
            unitsToRemove: []
        };
    }

    useEffect(() => {
        const fetchProductData = async () => {
            if (productInstanceId) {
                setFetchingProduct(true);
                const response = await apiService.procurements.products.get(transactionId, productInstanceId)
                if (!response.isSuccess) {
                    setSubmissionError('Failed to load product data. Please try again.');
                }
                else {
                    setExpirationDate(response.value.shelfLifeInDays != null ? new Date(Date.now() + response.value.shelfLifeInDays * 24 * 60 * 60 * 1000) : null)
                    setOriginalUnits(response.value.units || []);
                    setProductQuantity(response.value.quantity);
                    setOriginalQuantity(response.value.quantity)
                    reset(transformApiResponse(response.value))
                }
                setFetchingProduct(false);
            }
        };
        fetchProductData();
    }, [productInstanceId, reset, transactionId]);

    const onSubmit: SubmitHandler<TransactionProductFormData> = async (data) => {
        setLoading(true);
        setSubmissionError(null);
        try {
            data.transactionId = transactionId;

            if (isEditMode) {
                let currentUnits = getValues('units') || [];

                currentUnits = currentUnits.splice(0, parseInt(data.quantity))

                const unitsToAdd = currentUnits.filter(unit =>
                    !originalUnits.some((originalUnit: any) => originalUnit === unit.serialNumber)
                );

                const unitsToRemove: any = originalUnits
                    .filter((originalUnit: any) => !currentUnits.some(unit => unit.serialNumber === originalUnit))
                    .map(unit => unit);
                data.quantity = (parseInt(data.quantity) - originalQuantity).toString();
                data.unitsToAdd = unitsToAdd;
                data.unitsToRemove = unitsToRemove;
                data.units = undefined;
                await apiService.procurements.products.update(data);
            } else {
                await apiService.procurements.products.create(data);
            }
            console.log(isEditMode ? 'Product updated successfully' : 'Product added successfully');
            onSubmitSuccess();
        } catch (error: any) {
            console.error(error);
            setSubmissionError(error.message || "An unexpected error occurred. Please try again.");
        } finally {
            setLoading(false);
        }
    };

    const { data: productsResponse, error: productsError, isLoading: isLoadingProducts } = useProducts();

    const handleQuantityChange = (quantity: number) => {
        const productInstance = productsResponse?.value.find((p) => p.productInstanceId === productInstanceId);
        setProductQuantity(quantity)
        if (productInstance && productInstance.isTracked && !isNaN(quantity)) {
            const currentUnits = getValues('units') || [];
            const newUnits = Array(quantity).fill(null).map((_, index) =>
                currentUnits[index] || { serialNumber: '', expirationDate: undefined }
            );
            setValue(`units`, newUnits);
        }
    };

    const [serialErrors, setSerialErrors] = useState<string[]>([]);

    const serialNumbers = useWatch({
        control,
        name: `units`,
        defaultValue: Array.from({ length: productQuantity }).map(() => ({ serialNumber: '', expirationDate: undefined })),
    }) ?? [];

    useEffect(() => {
        const enteredSerialNumbers = serialNumbers?.map((unit: any) => unit.serialNumber);
        if (!enteredSerialNumbers) return
        const duplicates = enteredSerialNumbers.filter((serial: string, index: number) =>
            serial && enteredSerialNumbers.indexOf(serial) !== index
        );

        if (duplicates.length > 0) {
            setSerialErrors(duplicates);
        } else {
            setSerialErrors([]);
        }
    }, [serialNumbers]);


    return (
        <Box component="form" autoComplete="off" onSubmit={handleSubmit(onSubmit)}>

            <FormSelectField<TransactionProductFormData>
                name='productInstanceId'
                control={control}
                label="Product"
                error={!!errors?.productInstanceId}
                helperText={errors?.productInstanceId?.message ?? ''}
                disabled={productInstanceId != null}
                options={productsResponse?.value.map((product) => ({
                    value: product.productInstanceId,
                    label: product.name,
                    isTracked: (product.isWarranted || product.shelfLifeInDays != null),
                    expirationDate: product.shelfLifeInDays != null
                        ? new Date(Date.now() + product.shelfLifeInDays * 24 * 60 * 60 * 1000)
                        : undefined
                })) ?? []}
            />

            <FormField<TransactionProductFormData>
                name='unitPrice'
                control={control}
                rules={{ required: 'Unit Price is required', min: { value: 1, message: 'Unit Price must be at least 1' } }}
                label="Unit Price"
                isCurrency
                error={!!errors?.unitPrice}
                helperText={errors?.unitPrice?.message ?? ''}
            />

            <FormField<TransactionProductFormData>
                name='quantity'
                control={control}
                rules={{ required: 'Quantity is required', min: { value: 1, message: 'Quantity must be at least 1' } }}
                label="Quantity"
                isNumber
                error={!!errors?.quantity}
                helperText={errors?.quantity?.message ?? ''}
                onChange={(e) => handleQuantityChange(parseInt(e.target.value, 10))}
            />

            <Typography variant="subtitle1">Serial Numbers</Typography>
            {Array.from({ length: productQuantity }).map((_, serialIndex) => (
                <div key={serialIndex}>
                    <FormField<TransactionProductFormData>
                        name={`units.${serialIndex}.serialNumber`}
                        control={control}
                        rules={{ required: 'Serial Number is required' }}
                        label={`Serial Number ${serialIndex + 1}`}
                        error={!!errors?.units?.[serialIndex]?.serialNumber || serialErrors.includes(serialNumbers[serialIndex]?.serialNumber)}
                        helperText={errors?.units?.[serialIndex]?.serialNumber?.message || ''}
                    />
                    {serialErrors.includes(serialNumbers[serialIndex]?.serialNumber) && (
                        <FormHelperText error>
                            Serial number "{serialNumbers[serialIndex]?.serialNumber}" is duplicate!
                        </FormHelperText>
                    )}
                    {expirationDate && (
                        <FormDateField<TransactionProductFormData>
                            name={`units.${serialIndex}.expirationDate`}
                            control={control}
                            label={`Expiration Date for Serial ${serialIndex + 1}`}
                            error={false}
                            maxDate={expirationDate}
                        />
                    )}
                </div>
            ))}

            {submissionError && (
                <Typography color="error" sx={{ mt: 2 }}>
                    {submissionError}
                </Typography>
            )}

            <LoadingButton
                fullWidth
                size="large"
                type="submit"
                variant="contained"
                sx={{ mt: 3 }}
                loading={loading}
            >
                {isEditMode ? 'Update Product' : 'Add Product'}
            </LoadingButton>

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
