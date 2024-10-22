import { LoadingButton } from "@mui/lab";
import { Box, CircularProgress, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { SubmitHandler, useForm } from "react-hook-form";
import { FormField } from "src/components/form-fields/form-field";
import { apiService } from "src/services/api";

export interface PaymentFormData {
    transactionId: string;
    paymentId?: string;
    payedAmount: string;
    paymentMethod: string;
}

interface PaymentFormProps {
    transactionId: string;
    paymentId?: string
    onSubmitSuccess: () => void;
}

export function PaymentForm({ transactionId, paymentId, onSubmitSuccess }: PaymentFormProps) {
    const [loading, setLoading] = useState(false);
    const [submissionError, setSubmissionError] = useState<string | null>(null);
    const [fetchingPayment, setFetchingPayment] = useState(false);
    const isEditMode = !!paymentId;

    const { control, handleSubmit, reset, formState: { errors, dirtyFields } } = useForm<PaymentFormData>({
        defaultValues: {
            transactionId: transactionId,
            paymentId: paymentId,
            payedAmount: '',
            paymentMethod: ''
        },
    });

    useEffect(() => {
        const fetchPaymentData = async () => {
            if (paymentId) {
                setFetchingPayment(true);
                const response = await apiService.procurements.payments.get(transactionId, paymentId)
                if (!response.isSuccess) {
                    setSubmissionError('Failed to load payment data. Please try again.');
                }
                else {
                    reset(response.value)
                }
                setFetchingPayment(false);
            }
        };

        fetchPaymentData();
    }, [paymentId, reset]);

    const onSubmit: SubmitHandler<PaymentFormData> = async (data) => {
        setLoading(true);
        setSubmissionError(null);
        try {
            data.transactionId = transactionId;
            if (isEditMode) {
                await apiService.procurements.payments.update(data);
            } else {
                await apiService.procurements.payments.create(data);
            }
            console.log(isEditMode ? 'Payment updated successfully' : 'Payment added successfully');
            onSubmitSuccess();
        } catch (error: any) {
            console.error(error);
            setSubmissionError(error.message || "An unexpected error occurred. Please try again.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <Box component="form" autoComplete="off" onSubmit={handleSubmit(onSubmit)}>
            <FormField<PaymentFormData>
                name="payedAmount"
                control={control}
                label="Payed Amount"
                rules={{ required: 'Payed Amount is required' }}
                error={!!errors.payedAmount}
                isCurrency
                helperText={errors.payedAmount?.message}
            />

            <FormField<PaymentFormData>
                name="paymentMethod"
                control={control}
                label="Payment Method"
                rules={{ required: 'Payment Method is required' }}
                error={!!errors.paymentMethod}
                helperText={errors.paymentMethod?.message}
            />

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
                {isEditMode ? 'Update Payment' : 'Add Payment'}
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