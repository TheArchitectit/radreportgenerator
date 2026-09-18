import pandas as pd

# Open the Excel file
file_path = "docs/SizingWorkshop-RVTools.xlsx"

print("=" * 80)
print("RVTools Excel File Analysis")
print("=" * 80)

# Use pandas to get sheet names
xl_file = pd.ExcelFile(file_path)
print(f"\nTotal Worksheets: {len(xl_file.sheet_names)}")
print("Worksheet Names:")
for i, sheet_name in enumerate(xl_file.sheet_names):
    print(f"  {i+1}. {sheet_name}")

# Key sheets to analyze
key_sheets = ['vInfo', 'vHost', 'vDatastore', 'vDisk', 'vPartition', 'vNetwork', 'vSnapshot']

for sheet_name in xl_file.sheet_names:
    if sheet_name in key_sheets:
        print(f"\n{'=' * 80}")
        print(f"Sheet: {sheet_name}")
        print("=" * 80)
        
        try:
            # Read sheet with pandas to get better header/data handling
            df = pd.read_excel(file_path, sheet_name=sheet_name)
            
            print(f"\nColumns ({len(df.columns)}):")
            for i, col in enumerate(df.columns):
                dtype = df[col].dtype
                non_null_count = df[col].count()
                unique_count = df[col].nunique()
                sample_val = df[col].iloc[0] if non_null_count > 0 else "N/A"
                print(f"  {i+1}. {col:<30} | Type: {str(dtype):<15} | Non-null: {non_null_count} | Unique: {unique_count}")
            
            print(f"\nSample Data (first 3 rows):")
            print(df.head(3).to_string())
            
            print(f"\nDataFrame Info:")
            print(f"  Shape: {df.shape}")
            print(f"  Memory usage: {df.memory_usage(deep=True).sum() / 1024 / 1024:.2f} MB")
            
        except Exception as e:
            print(f"  Error reading sheet: {e}")

# General file information
print(f"\n{'=' * 80}")
print("File Information")
print("=" * 80)
print(f"File: {file_path}")
print(f"Excel Format: {'xlsx' if 'xlsx' in file_path else 'xls'}")