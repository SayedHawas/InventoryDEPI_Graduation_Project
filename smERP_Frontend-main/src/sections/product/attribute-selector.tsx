import React, { useState, useEffect, useCallback } from 'react';
import { Box, Button, Grid, IconButton, Typography } from '@mui/material';
import { Control, FieldErrors, useFieldArray, useForm, useWatch } from 'react-hook-form';
import { FormSelectField } from 'src/components/form-fields/form-select-field';
import { Iconify } from 'src/components/iconify';
import { AttributeValue } from 'src/services/types';
import { ProductInstanceFormData } from './product-instance-form';

export interface Attribute {
  value: number;
  label: string;
  values: { value: number; label: string }[];
}

interface AttributeSelectorProps {
  attributes: Attribute[];
  initialValues?: AttributeValue[];
  onAttributesChange: (selectedAttributes: AttributeValue[]) => void;
  setAttributesError: React.Dispatch<React.SetStateAction<string | null>>
  attributesError: string | null
  control: Control<any>;
  errors: FieldErrors<ProductInstanceFormData>
}

export const AttributeSelector: React.FC<AttributeSelectorProps> = ({ attributes, initialValues = [], onAttributesChange, setAttributesError, attributesError, control, errors }) => {
  // const { control } = useForm({
  //   defaultValues: {
  //     attributes: initialValues.length ? initialValues : [{ attributeId: '', attributeValueId: '' }],
  //   }
  // });

  const { fields, append, remove } = useFieldArray({ control, name: 'attributes' });
  const watchFieldArray = useWatch({ control, name: 'attributes' });

  const updateSelectedAttributes = useCallback(() => {

    const selectedAttributeIds = watchFieldArray?.map((field: AttributeValue) => field?.attributeId.toString()) || [];

    const uniqueIds = new Set(selectedAttributeIds);
    if (uniqueIds.size !== selectedAttributeIds.length) {
      setAttributesError('Attributes must be unique. You have selected one or more duplicate attributes.');
    } else {
      setAttributesError(null);
    }
    const selectedAttributes = watchFieldArray
      ?.filter((field: any) => field?.attributeId && field?.attributeValueId)
      .map((field: any) => ({
        attributeId: field.attributeId,
        attributeValueId: field.attributeValueId,
      })) || [];
    onAttributesChange(selectedAttributes);
  }, [watchFieldArray, onAttributesChange]);

  useEffect(() => {
    updateSelectedAttributes();
  }, [updateSelectedAttributes]);

  const handleAddAttribute = () => {
    append({ attributeId: '', attributeValueId: '' });
  };

  const getValueOptions = (index: number) => {
    const attributeId = watchFieldArray?.[index]?.attributeId;
    if (!attributeId) return [];
    const attribute = attributes.find(attr => attr.value.toString() === attributeId.toString());
    return attribute ? attribute.values.map(val => ({ value: (val as any).value, label: (val as any).label })) : [];
  };

  return (
    <>
      <Box>
        {fields.map((field, index) => (
          <Grid key={field.id} sx={{ display: 'flex', gap: 2, mb: 2, alignItems: 'center' }}>
            <FormSelectField
              name={`attributes.${index}.attributeId`}
              control={control}
              label="Attribute"
              options={attributes.map(attr => ({
                value: attr.value.toString(),
                label: attr.label
              }))}
              rules={{ required: 'Attribute is required' }}
              error={!!errors.attributes?.[index]?.attributeId}
              helperText={errors.attributes?.[index]?.attributeId?.message || ''}
            />
            <FormSelectField
              name={`attributes.${index}.attributeValueId`}
              control={control}
              label="Value"
              options={getValueOptions(index)}
              rules={{ required: 'Value is required' }}
              error={!!errors.attributes?.[index]?.attributeValueId}
              helperText={errors.attributes?.[index]?.attributeValueId?.message || ''}
            />
            <IconButton onClick={() => remove(index)}>
              <Iconify icon={'mingcute:delete-2-line'} />
            </IconButton>
          </Grid>
        ))}

        <Button
          onClick={handleAddAttribute}
          variant="contained"
          disabled={!!attributesError}
        >
          Add Attribute
        </Button>
        {attributesError && (
          <Typography color="error" sx={{ mt: 2 }}>
            {attributesError}
          </Typography>
        )}
      </Box>
    </>
  );
};
