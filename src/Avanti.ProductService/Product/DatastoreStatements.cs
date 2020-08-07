namespace Avanti.ProductService.Product
{
    public static class DataStoreStatements
    {
        private const string Schema = "product";
        private const string ProductTable = "product";
        private const string SampleDataTable = "sampledata";

        public static string GetProductById => $@"
            SELECT productJson FROM {Schema}.{ProductTable}
            WHERE id = @Id
        ";

        public static string GetProductsById => $@"
            SELECT id, productJson FROM {Schema}.{ProductTable}
            WHERE id = ANY(@Ids)
        ";

        public static string InsertProduct => $@"
            INSERT INTO {Schema}.{ProductTable} (productJson, created, updated)
            VALUES (json_in(@ProductJson::cstring), @Now, @Now)
            RETURNING id
        ";

        public static string UpdateProduct => $@"
            UPDATE {Schema}.{ProductTable} SET (productJson, updated) = (json_in(@ProductJson::cstring), @Now)
            WHERE id = @Id
            RETURNING id
        ";

        public static string GetSampleDataStatus => $@"
            SELECT * FROM {Schema}.{SampleDataTable}
            LIMIT 1
        ";

        public static string InsertSampleDataStatus => $@"
            INSERT INTO {Schema}.{SampleDataTable} (executed) VALUES (@Now)
        ";
    }
}
