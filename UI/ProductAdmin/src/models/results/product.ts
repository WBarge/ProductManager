import { ProductOption } from "./ProductOption";

export class Product {
    idValue!: string;
    name!: string;
    shortDescription!: string;
    sku!: string;
    price!: number;
    description!: string;
    options!: ProductOption[];
}
