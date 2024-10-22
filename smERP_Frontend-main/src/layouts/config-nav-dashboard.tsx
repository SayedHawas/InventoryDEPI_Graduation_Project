import { Label } from 'src/components/label';
import { SvgColor } from 'src/components/svg-color';

const icon = (name: string) => (
  <SvgColor width="100%" height="100%" src={`/assets/icons/navbar/${name}.svg`} />
);

export const navData = [
  {
    title: 'Dashboard',
    path: '/',
    icon: icon('ic-analytics'),
  },
  {
    title: 'User',
    path: '/user',
    icon: icon('ic-user'),
    roles: ['Admin', 'Branch Manager'],
  },
  {
    title: 'Branch',
    path: '/branch',
    icon: icon('ic-branch'),
    roles: ['Admin'],
  },
  {
    title: 'Storage Location',
    path: '/storage-location',
    icon: icon('ic-storage-location'),
    roles: ['Admin', 'Branch Manager'],
  },
  {
    title: 'Brand',
    path: '/brand',
    icon: icon('ic-brand'),
  },
  {
    title: 'Category',
    path: '/category',
    icon: icon('ic-category'),
  },
  {
    title: 'Attribute',
    path: '/attribute',
    icon: icon('ic-attribute'),
  },
  {
    title: 'Product',
    path: '/products',
    icon: icon('ic-cart'),
  },
  {
    title: 'Supplier',
    path: '/supplier',
    icon: icon('ic-supplier'),
  },
  {
    title: 'Transactions',
    path: '/transactions',
    icon: icon('ic-transaction'),
    subItems: [
      {
        title: 'Procurement',
        path: '/transactions/procurement',
      },
      {
        title: 'Sales',
        path: '/transactions/sales',
      },
      {
        title: 'Adjustment',
        path: '/transactions/adjustment',
      },
    ],
  },
];