import { TestData } from "../test-data";
import { Product } from "./product";

export class ProductsListResult {
  public data!:Product[];
  public totalRecordSize!:number;
}
