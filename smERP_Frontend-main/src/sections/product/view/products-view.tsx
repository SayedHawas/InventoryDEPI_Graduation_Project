import { Box, Typography, Button, Card, TableContainer, CircularProgress, TableRow, TableCell, Table, TableBody, TableHead, SelectChangeEvent } from "@mui/material";
import { useState, useCallback } from "react";
import { Iconify } from "src/components/iconify";
import { Label } from "src/components/label";
import { Scrollbar } from "src/components/scrollbar";
import { useEntities } from "src/hooks/use-entities";
import { useTable } from "src/hooks/use-table";
import { CustomDialog } from "src/layouts/components/custom-dialog";
import { GenericTable, TableAction } from "src/layouts/components/table/generic-table";
import { DashboardContent } from "src/layouts/dashboard";
import { AttributeForm } from "src/sections/attribute/attribute-form";
import { TableColumn } from "src/services/types";
import { ProductForm } from "../product-form";
import { GenericTableRow } from "src/layouts/components/table/generic-table-row";
import { ProductInstanceForm } from "../product-instance-form";
import { useBrands } from "src/hooks/use-brands";
import { useParentCategories, useProductCategories } from "src/hooks/use-categories";
import SelectInput from "@mui/material/Select/SelectInput";
import { FormSelectField } from "src/components/form-fields/form-select-field";

type ProductProps = {
  id: string,
  name: string,
  modelNumber: string,
  isTracked: string,
  shelfLifeInDays?: string,
  warrantyInDays?: string,
  brand: string,
  category: string
  instances?: ProductInstanceProps[]
}

type ProductInstanceProps = {
  id: string,
  sku: string,
  qty: number,
  buyPrice: number,
  sellPrice: number,
  image?: string
}

const transformProduct = (apiProduct: any): ProductProps => {
  return {
    id: apiProduct.productId,
    name: apiProduct.name,
    modelNumber: apiProduct.modelNumber,
    isTracked: apiProduct.isTracked ? 'Yes' : 'No',
    shelfLifeInDays: apiProduct.shelfLifeInDays ? apiProduct.shelfLifeInDays + ' Days' : 'Does not have shelf life',
    warrantyInDays: apiProduct.warrantyInDays ? apiProduct.warrantyInDays + ' Days' : 'Does not have warranty',
    brand: apiProduct.brand,
    category: apiProduct.category,
    instances: apiProduct.instances?.map((instance: any) => ({
      id: instance.productInstanceId,
      sku: instance.sku,
      qty: instance.quantityInStock,
      buyPrice: instance.buyingPrice,
      sellPrice: instance.sellingPrice,
      image: instance.image ?? "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRGh5WFH8TOIfRKxUrIgJZoDCs1yvQ4hIcppw&s"
    }))
  };
};

const PRODUCT_TABLE_COLUMNS: TableColumn<ProductProps>[] = [
  { id: 'name', label: 'Name' },
  { id: 'modelNumber', label: 'Model Number' },
  { id: 'brand', label: 'Brand' },
  { id: 'category', label: 'Category' },
  { id: 'shelfLifeInDays', label: 'Shelf Life' },
  { id: 'warrantyInDays', label: 'Warranty' },
  {
    id: 'isTracked',
    label: 'Tracked',
    align: 'center',
    render: (product) => (
      <Label color={(product.isTracked === 'No' && 'error') || 'success'}>{product.isTracked}</Label>
    ),
  },
];

export function ProductsView() {
  const [filterName, setFilterName] = useState('');
  const [filterCategory, setFilterCategory] = useState('')
  const [filterBrand, setFilterBrand] = useState('')
  const [filterDateStart, setFilterDateStart] = useState<Date | null>(null)
  const [filterDateEnd, setFilterDateEnd] = useState<Date | null>(null)
  const [showProductForm, setShowProductForm] = useState(false);
  const [showProductInstanceForm, setShowProductInstanceForm] = useState(false);
  const [showAttributeForm, setShowAttributeForm] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState<ProductProps | null>(null);
  const [selectedProductInstance, setSelectedProductInstance] = useState<ProductInstanceProps | null>(null);

  const table = useTable();

  const { entities: products, loading, error, totalCount, updateParams, refetch } = useEntities<ProductProps>(
    'products',
    {
      PageNumber: table.page + 1,
      PageSize: table.rowsPerPage,
      SortBy: table.orderBy,
      SortDescending: table.order === 'desc',
      SearchTerm: filterName,
      FilterBy: [{ filterBy: "brandId", value: filterBrand }, { filterBy: "categoryId", value: filterCategory }, { filterBy: "startDate", value: filterDateStart?.toDateString() ?? '' }, { filterBy: "endDate", value: filterDateEnd?.toDateString() ?? '' }]
    },
    transformProduct
  );

  const handleFilterName = (event: React.ChangeEvent<HTMLInputElement>) => {
    const newFilterName = event.target.value;
    setFilterName(newFilterName);
    updateParams({ SearchTerm: newFilterName, PageNumber: 1 });
    table.onChangePage(null, 0);
  };

  const handleBrandChange = (value: string, event: SelectChangeEvent<unknown>) => {
    setFilterBrand(value);
    updateParams({
      FilterBy: [
        { filterBy: "brandId", value: value },
        { filterBy: "categoryId", value: filterCategory },
        { filterBy: "startDate", value: filterDateStart?.toDateString() ?? '' },
        { filterBy: "endDate", value: filterDateEnd?.toDateString() ?? '' }
      ],
      PageNumber: 1
    });
    table.onChangePage(null, 0);
  };

  const handleCategoryChange = (value: string, event: SelectChangeEvent<unknown>) => {
    setFilterCategory(value);
    updateParams({
      FilterBy: [
        { filterBy: "categoryId", value: value },
        { filterBy: "brandId", value: filterBrand },
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

  const handleSort = (property: keyof ProductProps) => {
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

  const handleAddAttribute = () => {
    //setSelectedProduct(null);
    setShowAttributeForm(true);
  };

  const handleAddProduct = () => {
    setSelectedProduct(null);
    setShowProductForm(true);
  };

  const handleEditProduct = (product: ProductProps) => {
    setSelectedProduct(product);
    setShowProductForm(true);
  };

  const handleProductFormClose = useCallback(() => {
    setShowProductForm(false);
    refetch();
  }, [refetch]);

  const handleProductFormCancel = () => {
    setShowProductForm(false);
  }

  const handleAttributeFormClose = () => {
    setShowAttributeForm(false);
  }

  const handleAttributeFormCancel = () => {
    setShowAttributeForm(false);
  }

  const handleAddInstance = (product: ProductProps) => {
    setSelectedProduct(product);
    setSelectedProductInstance(null)
    setShowProductInstanceForm(true);
  }

  const handleEditInstance = (product: ProductProps, instance: ProductInstanceProps) => {
    setSelectedProduct(product);
    setSelectedProductInstance(instance)
    setShowProductInstanceForm(true);
  }

  const handleProductInstanceFormClose = useCallback(() => {
    setShowProductInstanceForm(false);
    refetch();
  }, [refetch]);

  const handleProductInstanceFormCancel = () => {
    setShowProductInstanceForm(false);
  }

  const { data: brands } = useBrands();
  let { data: categories } = useParentCategories();
  { data: categories } useProductCategories();

  const tableActions: TableAction<ProductProps>[] = [
    {
      label: 'Add Product Instance',
      icon: 'mingcute:add-fill',
      onClick: (row) => handleAddInstance(row),
    },
    {
      label: 'Edit',
      icon: 'solar:pen-bold',
      onClick: (row) => handleEditProduct(row),
    }
  ];

  const childTableActions: TableAction<ProductInstanceProps>[] = [
    {
      label: 'Edit',
      icon: 'solar:pen-bold',
      onClick: (row, product) => handleEditInstance(product, row),
    }
  ]

  return (
    <DashboardContent>
      <Box display="flex" alignItems="center" mb={5} flexDirection={{ xs: 'column', sm: 'row' }} gap={1}>
        <Typography variant="h4" flexGrow={1}>
          Products
        </Typography>
        <Button variant="contained" color="inherit" onClick={handleAddAttribute} startIcon={<Iconify icon="mingcute:add-line" />}>
          New attribute
        </Button>
        <Button variant="contained" color="inherit" onClick={handleAddProduct} startIcon={<Iconify icon="mingcute:add-line" />}>
          New product
        </Button>
      </Box>

      <Card>
        <TableContainer sx={{ position: 'relative', overflow: 'unset' }}>
          <GenericTable
            data={products}
            columns={PRODUCT_TABLE_COLUMNS}
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
            onSelectAllRows={(checked) => table.onSelectAllRows(checked, products.map(product => product.id))}
            onSelectRow={handleSelectRow}
            getRowId={(row: ProductProps) => row.id}
            actions={tableActions}
            customFilters={
              <>
                <FormSelectField
                  label="Brand"
                  options={[
                    { value: '', label: 'All Brands' },
                    ...(brands?.value ?? [])
                  ]}
                  onChange={handleBrandChange}
                  defaultValue=""
                />

                <FormSelectField
                  label="Category"
                  options={[
                    { value: '', label: 'All Categories' },
                    ...(categories?.value ?? [])
                  ]}
                  onChange={handleCategoryChange}
                  defaultValue=""
                />
              </>
            }
            expandableContent={(product: ProductProps) => (
              <>
                <Typography variant="h6" gutterBottom component="div">
                  Instances
                </Typography>
                <Table size="small" aria-label="instances">
                  <TableHead>
                    <TableRow>
                      <TableCell>SKU</TableCell>
                      <TableCell>Quantity</TableCell>
                      <TableCell>Buy Price</TableCell>
                      <TableCell>Sell Price</TableCell>
                      <TableCell align="center">Image</TableCell>
                      <TableCell />
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {product.instances?.map((instance) => (
                      <GenericTableRow<ProductInstanceProps, ProductProps>
                        key={instance.id}
                        row={instance}
                        columns={[
                          { id: 'sku', label: 'SKU', align: 'left' },
                          { id: 'qty', label: 'Quantity', align: 'left' },
                          { id: 'buyPrice', label: 'Buy Price', align: 'left' },
                          { id: 'sellPrice', label: 'Sell Price', align: 'left' },
                          {
                            id: 'image', label: 'Image', align: 'center', render: () => (
                              <img src={instance.image} alt={instance.sku} width="100" />
                            )
                          },
                        ]}
                        selected={false}
                        getRowId={(row) => row.id}
                        actions={childTableActions}
                        actionContext={product}
                        expandable={false}
                      />
                    ))}
                  </TableBody>
                </Table>
              </>
            )} />
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
      <CustomDialog open={showProductForm} handleCancel={handleProductFormCancel} title={selectedProduct?.id ? 'Edit product' : 'Add new product'} content={<ProductForm productId={selectedProduct?.id} onSubmitSuccess={handleProductFormClose} />} />
      <CustomDialog open={showProductInstanceForm} handleCancel={handleProductInstanceFormCancel} title={selectedProductInstance?.id ? 'Edit product instance' : 'Add new product instance'} content={<ProductInstanceForm productId={selectedProduct?.id ?? ''} productInstanceId={selectedProductInstance?.id} onSubmitSuccess={handleProductInstanceFormClose} />} />
      <CustomDialog open={showAttributeForm} handleCancel={handleAttributeFormCancel} title={'Add new attribute'} content={<AttributeForm onSubmitSuccess={handleAttributeFormClose} />} />
    </DashboardContent>
  )
}