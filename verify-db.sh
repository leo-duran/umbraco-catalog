#!/bin/bash

# Create output directory if it doesn't exist
mkdir -p ./docs/verification

# Get current timestamp for the output file
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
OUTPUT_FILE="./docs/verification/db_verification_${TIMESTAMP}.md"

echo "# Umbraco Database Verification - $(date)" > $OUTPUT_FILE
echo "" >> $OUTPUT_FILE

echo "## Document Types" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
sqlite3 src/Catalog/umbraco/Data/Umbraco.sqlite.db "SELECT un.id, un.text, ct.alias, ct.icon FROM umbracoNode un JOIN cmsContentType ct ON un.id = ct.nodeId WHERE un.nodeObjectType = 'A2CB7800-F571-4787-9638-BC48539A0EFB';" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
echo "" >> $OUTPUT_FILE

echo "## Templates" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
sqlite3 src/Catalog/umbraco/Data/Umbraco.sqlite.db "SELECT un.id, un.text, t.alias FROM umbracoNode un JOIN cmsTemplate t ON un.id = t.nodeId;" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
echo "" >> $OUTPUT_FILE

echo "## Property Groups" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
sqlite3 src/Catalog/umbraco/Data/Umbraco.sqlite.db "SELECT ptg.id, ptg.text, ptg.alias, ct.alias as contentTypeAlias FROM cmsPropertyTypeGroup ptg JOIN cmsContentType ct ON ptg.contenttypeNodeId = ct.nodeId ORDER BY ct.alias;" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
echo "" >> $OUTPUT_FILE

echo "## Properties" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
sqlite3 src/Catalog/umbraco/Data/Umbraco.sqlite.db "SELECT pt.id, pt.Name, pt.Alias, ct.alias as contentTypeAlias FROM cmsPropertyType pt JOIN cmsContentType ct ON pt.contentTypeId = ct.nodeId ORDER BY ct.alias, pt.sortOrder;" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
echo "" >> $OUTPUT_FILE

echo "## Template Files" >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE
find src/Catalog/Views -name "*.cshtml" | sort >> $OUTPUT_FILE
echo '```' >> $OUTPUT_FILE

echo "Database verification completed. Results saved to $OUTPUT_FILE" 