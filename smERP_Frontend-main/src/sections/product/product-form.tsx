import { LoadingButton } from "@mui/lab";
import { Box, CircularProgress, Grid, Typography } from "@mui/material";
import { useState, useEffect } from "react";
import { useForm, SubmitHandler } from "react-hook-form";
import { FormField } from "src/components/form-fields/form-field";
import { FormSelectField } from "src/components/form-fields/form-select-field";
import { useBrands } from "src/hooks/use-brands";
import { useProductCategories } from "src/hooks/use-categories";
import { apiService } from "src/services/api";

export interface ProductFormData {
    productId?: string;
    englishName: string;
    arabicName: string;
    modelNumber: string;
    brandId: string;
    categoryId: string;
    description: string;
    shelfLifeInDays: string;
    warrantyInDays: string;
}

interface ProductFormProps {
    productId?: string;
    onSubmitSuccess: () => void;
}

export function ProductForm({ productId, onSubmitSuccess }: ProductFormProps) {
    const [loading, setLoading] = useState(false);
    const [submissionError, setSubmissionError] = useState<string | null>(null);
    const [fetchingProduct, setFetchingProduct] = useState(false);
    const isEditMode = !!productId;

    const { control, handleSubmit, reset, formState: { errors, dirtyFields } } = useForm<ProductFormData>({
        defaultValues: {
            productId: '',
            englishName: '',
            arabicName: '',
            modelNumber: '',
            brandId: '',
            categoryId: '',
            description: '',
            shelfLifeInDays: '',
            warrantyInDays: '',
        },
    });

    useEffect(() => {
        const fetchProductData = async () => {
            if (productId) {
                setFetchingProduct(true);
                try {
                    const response = await fetch(`https://smerp.runasp.net/products/${productId}`);
                    if (!response.ok) {
                        throw new Error('Failed to fetch product data');
                    }
                    const responseBody = await response.json();
                    const productData: ProductFormData = responseBody.value;
                    productData.shelfLifeInDays = productData.shelfLifeInDays ?? '';
                    productData.warrantyInDays = productData.warrantyInDays ?? ''
                    reset(productData);
                } catch (error) {
                    console.error('Error fetching product data:', error);
                    setSubmissionError('Failed to load product data. Please try again.');
                } finally {
                    setFetchingProduct(false);
                }
            }
        };

        fetchProductData();
    }, [productId, reset]);

    const onSubmit: SubmitHandler<ProductFormData> = async (data) => {
        setLoading(true);
        setSubmissionError(null);
        try {
            if (isEditMode) {
                const changedData = Object.keys(dirtyFields).reduce((acc: Partial<ProductFormData>, key) => {
                    acc[key as keyof ProductFormData] = data[key as keyof ProductFormData];
                    return acc;
                }, {});
                await apiService.products.update(productId, changedData);
            } else {
                await apiService.products.create(data);
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

    const { data: brandsResponse, error: brandsError, isLoading: isLoadingBrands } = useBrands();
    const { data: categoriesResponse, error: categoriesError, isLoading: isLoadingCategories } = useProductCategories();

    if (isLoadingBrands || isLoadingCategories || fetchingProduct) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" height="100vh">
                <CircularProgress />
                <Typography variant="h6" sx={{ marginLeft: 2 }}>
                    Loading...
                </Typography>
            </Box>
        );
    }

    if (brandsError || categoriesError) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" height="100vh">
                <Typography color="error">{brandsError?.message} {categoriesError?.message}</Typography>
            </Box>
        );
    }

    return (
        <Box component="form" autoComplete="on" onSubmit={handleSubmit(onSubmit)}>
            <Grid container spacing={2}>
                <Grid item xs={12} sm={6}>
                    <FormField<ProductFormData>
                        name="englishName"
                        control={control}
                        label="English Name"
                        rules={{ required: 'English name is required' }}
                        error={!!errors.englishName}
                        helperText={errors.englishName?.message}
                    />
                </Grid>
                <Grid item xs={12} sm={6}>
                    <FormField<ProductFormData>
                        name="arabicName"
                        control={control}
                        label="Arabic Name"
                        rules={{ required: 'Arabic name is required' }}
                        error={!!errors.arabicName}
                        helperText={errors.arabicName?.message}
                    />
                </Grid>
                <Grid item xs={12} sm={6}>
                    <FormField<ProductFormData>
                        name="modelNumber"
                        control={control}
                        label="Model Number"
                        rules={{ required: 'Model number is required' }}
                        error={!!errors.modelNumber}
                        helperText={errors.modelNumber?.message}
                    />
                </Grid>
                <Grid item xs={12} sm={6}>
                    <FormField<ProductFormData>
                        name="description"
                        control={control}
                        label="Description"
                        rules={{ required: 'Description is required' }}
                        error={!!errors.description}
                        helperText={errors.description?.message}
                    />
                </Grid>
                <Grid item xs={12} sm={6}>
                    <FormField<ProductFormData>
                        name="shelfLifeInDays"
                        control={control}
                        label="Shelf Life (in days)"
                        isNumber={true}
                        error={!!errors.shelfLifeInDays}
                        helperText={errors.shelfLifeInDays?.message}
                    />
                </Grid>
                <Grid item xs={12} sm={6}>
                    <FormField<ProductFormData>
                        name="warrantyInDays"
                        control={control}
                        label="Warranty (in days)"
                        isNumber={true}
                        error={!!errors.warrantyInDays}
                        helperText={errors.warrantyInDays?.message}
                    />
                </Grid>
                <Grid item xs={12} sm={6}>
                    <Typography variant="subtitle1" gutterBottom sx={{ mt: 2 }}>
                        Brand
                    </Typography>
                    <FormSelectField<ProductFormData>
                        name="brandId"
                        control={control}
                        rules={{ required: 'Brand is required' }}
                        error={!!errors.brandId}
                        helperText={errors.brandId?.message}
                        options={brandsResponse?.value || []}
                    />
                </Grid>
                <Grid item xs={12} sm={6}>
                    <Typography variant="subtitle1" gutterBottom sx={{ mt: 2 }}>
                        Category
                    </Typography>
                    <FormSelectField<ProductFormData>
                        name="categoryId"
                        control={control}
                        rules={{ required: 'Category is required' }}
                        error={!!errors.categoryId}
                        helperText={errors.categoryId?.message}
                        options={categoriesResponse?.value || []}
                    />
                </Grid>
            </Grid>
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
    );
}
