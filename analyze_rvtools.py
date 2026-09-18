import sys
import os
sys.path.append(os.path.dirname(os.path.abspath(__file__)))

import pandas as pd

def analyze_rvtools_excel():
    file_path = "docs/SizingWorkshop-RVTools.xlsx"
    
    try:
        # Check if file exists
        if not os.path.exists(file_path):
            print(f"Error: File not found at {file_path}")
            return
        
        print("=" * 80)
        print("RVTools Excel File Analysis")
        print("=" * 80)
        print(f"File: {file_path}")
        print(f"File size: {os.path.getsize(file_path)} bytes")
        
        # Read Excel file
        df_dict = pd.read_excel(file_path, sheet_name=None)
        
        # Worksheet names
        print(f"\nTotal worksheets: {len(df_dict)}")
        print("Worksheet names:")
        for i, sheet_name in enumerate(df_dict.keys(), 1):
            print(f"  {i}. {sheet_name}")
        
        # Key sheets to analyze
        key_sheets = ['vInfo', 'vHost', 'vDatastore', 'vDisk', 'vPartition', 'vNetwork', 'vSnapshot']
        
        for sheet_name, df in df_dict.items():
            if sheet_name in key_sheets:
                print(f"\n{'=' * 80}")
                print(f"Sheet: {sheet_name}")
                print(f"{'=' * 80}")
                
                # Worksheet dimensions
                print(f"Dimensions: {df.shape[0]} rows x {df.shape[1]} columns")
                print(f"Memory usage: {df.memory_usage(deep=True).sum() / 1024 / 1024:.2f} MB")
                
                # Column analysis
                print(f"\nColumns ({len(df.columns)}):")
                
                for i, col in enumerate(df.columns, 1):
                    dtype = df[col].dtype
                    non_null_count = df[col].count()
                    unique_count = df[col].nunique()
                    
                    # Get sample value
                    try:
                        sample_val = df[col].iloc[0] if non_null_count > 0 else "N/A"
                        if pd.isna(sample_val):
                            sample_val = "N/A"
                        else:
                            # Truncate long strings
                            sample_str = str(sample_val)
                            if len(sample_str) > 40:
                                sample_val = sample_str[:37] + "..."
                            else:
                                sample_val = sample_str
                    except:
                        sample_val = "N/A"
                    
                    print(f"  {i:2d}. {col:<35} | Type: {str(dtype):<12} | Non-null: {non_null_count:4d} | Unique: {unique_count:3d} | Eg: {sample_val}")
                
                # Sample data (first 3 rows)
                print(f"\nSample Data (first 3 rows):")
                print(df.head(3).to_string())
                
                # Data type summary
                print(f"\nData Types Summary:")
                dtype_counts = df.dtypes.value_counts()
                for dtype, count in dtype_counts.items():
                    print(f"  {str(dtype):<15}: {count} columns")
                
                # Missing data
                missing_data = df.isnull().sum()
                if missing_data.sum() > 0:
                    print(f"\nMissing Data:")
                    for col, missing_count in missing_data[missing_data > 0].items():
                        print(f"  {col}: {missing_count} ({missing_count/len(df)*100:.1f}%)")
        
        print(f"\n{'=' * 80}")
        print("Analysis Complete")
        print("=" * 80)
        
    except Exception as e:
        print(f"Error analyzing file: {e}")
        import traceback
        traceback.print_exc()

if __name__ == "__main__":
    analyze_rvtools_excel()