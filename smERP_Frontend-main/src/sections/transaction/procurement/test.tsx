import React, { useCallback, useEffect, useRef, useState } from 'react';
import { Control, useFieldArray, UseFormSetValue, useWatch } from 'react-hook-form';
import { Grid, Button, Typography, FormHelperText, IconButton } from '@mui/material';
import { FormSelectField } from 'src/components/form-fields/form-select-field';
import { FormField } from 'src/components/form-fields/form-field';
import { FormDateField } from 'src/components/form-fields/form-date-field';
import { Iconify } from 'src/components/iconify';

interface ProductOption {
  value: string;
  label: string;
  isTracked: boolean;
  expirationDate?: Date
}

interface ProductEntryProps {
  control: Control<any>;
  index: number;
  productOptions: ProductOption[];
  onProductChange: (index: number, productInstanceId: string) => void;
  onQuantityChange: (index: number, quantity: number) => void;
  onRemove: () => void;
  errors: any;
}

const ProductEntry: React.FC<ProductEntryProps> = ({
  control,
  index,
  productOptions,
  onProductChange,
  onQuantityChange,
  onRemove,
  errors,
}) => {
  return (
    <Grid sx={{ display: 'flex', gap: 2, mb: 2, alignItems: 'center', flexWrap: 'wrap' }}>

        <FormSelectField
          name={`products.${index}.productInstanceId`}
          control={control}
          label="Product"
          error={!!errors?.products?.[index]?.productInstanceId}
          helperText={errors?.products?.[index]?.productInstanceId?.message ?? ''}
          options={productOptions}
          onChange={(e) => onProductChange(index, e)}
          rules={{
            required: 'Product is required',
            validate: (value, formValues) => {
              const products = formValues.products || [];
              const count = products.filter((p: any) => p.productInstanceId === value).length;
              return count <= 1 || 'This product has already been selected';
            }
          }}
        />

        <FormField
          name={`products.${index}.quantity`}
          control={control}
          rules={{ required: 'Quantity is required', min: { value: 1, message: 'Quantity must be at least 1' } }}
          label="Quantity"
          isNumber
          error={!!errors?.products?.[index]?.quantity}
          helperText={errors?.products?.[index]?.quantity?.message ?? ''}
          onChange={(e) => onQuantityChange(index, parseInt(e.target.value, 10))}
        />

        <FormField
          name={`products.${index}.unitPrice`}
          control={control}
          rules={{ required: 'Unit Price is required', min: { value: 1, message: 'Unit Price must be at least 1' } }}
          label="Unit Price"
          isCurrency
          error={!!errors?.products?.[index]?.unitPrice}
          helperText={errors?.products?.[index]?.unitPrice.message ?? ''}
        />

        <IconButton onClick={onRemove}>
          <Iconify icon={'mingcute:delete-2-line'} />
        </IconButton>

    </Grid>
  );
};

interface ProductUnitsProps {
  control: Control<any>;
  productIndex: number;
  quantity: number;
  expirationDate?: Date;
}

// In ProductUnits component
const ProductUnits: React.FC<ProductUnitsProps> = ({ control, productIndex, quantity, expirationDate }) => {
  const [serialErrors, setSerialErrors] = useState<string[]>([]);

  const serialNumbers = useWatch({
    control,
    name: `products.${productIndex}.units`,
    defaultValue: Array.from({ length: quantity }).map(() => ({ serialNumber: '', expirationDate: null })),
  });

  useEffect(() => {
    const enteredSerialNumbers = serialNumbers.map((unit: any) => unit.serialNumber);
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
    <Grid item xs={12}>
      <Typography variant="subtitle1">Serial Numbers</Typography>
      {Array.from({ length: quantity }).map((_, serialIndex) => (
        <div key={serialIndex}>
          <FormField
            name={`products.${productIndex}.units.${serialIndex}.serialNumber`}
            control={control}
            label={`Serial Number ${serialIndex + 1}`}
            error={serialErrors.includes(serialNumbers[serialIndex]?.serialNumber)}
            helperText=''
          />
          {serialErrors.includes(serialNumbers[serialIndex]?.serialNumber) && (
            <FormHelperText error>
              Serial number "{serialNumbers[serialIndex]?.serialNumber}" is duplicate!
            </FormHelperText>
          )}
          {expirationDate && (
            <FormDateField
              name={`products.${productIndex}.units.${serialIndex}.expirationDate`}
              control={control}
              label={`Expiration Date for Serial ${serialIndex + 1}`}
              error={false}
              maxDate={expirationDate}
            />
          )}
        </div>
      ))}
    </Grid>
  );
};

interface ProductsFormProps {
  control: Control<any>;
  setValue: UseFormSetValue<any>;
  errors: any;
  productOptions: ProductOption[];
}

const ProductsForm: React.FC<ProductsFormProps> = ({
  control,
  setValue,
  errors,
  productOptions,
}) => {
  console.log(productOptions)

  const { fields, append, remove } = useFieldArray({
    control,
    name: 'products',
  });

  const watchFieldArray = useWatch({
    control,
    name: 'products',
    defaultValue: fields,
  });

  const prevProductsRef = useRef<any[]>([]);

  console.log(fields)

  const controlledFields = fields.map((field, index) => ({
    ...field,
    ...watchFieldArray[index],
  }));

  const updateSelectedProducts = useCallback(() => {
    const selectedProductIds = watchFieldArray?.map((field: any) => field?.productInstanceId) || [];
    const uniqueIds = new Set(selectedProductIds.filter(Boolean));

    if (uniqueIds.size !== selectedProductIds.filter(Boolean).length) {
      watchFieldArray.forEach((field: any, index: any) => {
        if (field.productInstanceId && prevProductsRef.current[index]?.productInstanceId !== field.productInstanceId) {
          setValue(`products.${index}.productInstanceId`, field.productInstanceId, { shouldValidate: true });
        }
      });
    }

    prevProductsRef.current = watchFieldArray;
  }, [watchFieldArray, setValue]);

  useEffect(() => {
    updateSelectedProducts();
  }, [updateSelectedProducts]);

  const handleAddProduct = () => {
    append({ productInstanceId: '', quantity: '', unitPrice: '', units: [{ serialNumber: '' }] });
  };

  const handleProductChange = (index: number, productInstanceId: string) => {
    // Implement product change logic here
  };

  const handleQuantityChange = (index: number, quantity: number) => {
    const productInstanceId = (fields[index] as any).productInstanceId;
    const productInstance = productOptions.find((p) => p.value === productInstanceId);
    if (productInstance && productInstance.isTracked && !isNaN(quantity)) {
      const units = Array(quantity).fill({ serialNumber: '' });
      setValue(`products.${index}.units`, units);
    }
  };

  const isTracked = (productInstanceId: any) => {
    const productInstance = productOptions.find((p) => p.value === productInstanceId);
    if (!productInstance) return false
    if (productInstance.isTracked) return true
    return false
  }

  return (
    <>
      {controlledFields.map((field, index) => (
        <React.Fragment key={field.id}>
          <ProductEntry
            control={control}
            index={index}
            productOptions={productOptions}
            onProductChange={handleProductChange}
            onQuantityChange={handleQuantityChange}
            onRemove={() => remove(index)}
            errors={errors}
          />
          {field.productInstanceId && isTracked(field.productInstanceId) && (Number(field.quantity) > 0) && (
            <ProductUnits
              control={control}
              productIndex={index}
              quantity={Number(field.quantity)}
              expirationDate={productOptions.find((p) => p.value === field.productInstanceId)?.expirationDate}
            />
          )}
        </React.Fragment>
      ))}
      <Button onClick={handleAddProduct}>Add Product</Button>
    </>
  );
};

export default ProductsForm;