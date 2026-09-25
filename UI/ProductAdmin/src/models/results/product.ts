import { Characteristic } from "./characteristic";
import { ProductCharacteristic } from "./product-characteristic";
import { ProductOption } from "./ProductOption";

export class Product {
    idValue!: string;
    name!: string;
    shortDescription!: string;
    sku!: string;
    price!: number;
    cost!:number;
    estimated!:number;
    description!: string;
    options!: ProductOption[];
    characteristics!:ProductCharacteristic[];
}
