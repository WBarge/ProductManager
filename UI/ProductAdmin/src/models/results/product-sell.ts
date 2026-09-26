export class ProductSell {
    id!: string; //<- only used to the backend, not used in the UI
    idValue!: string;
    productId!: string;
    start!:Date;
    end!: Date;
    price!:number;
}
