/**
* Verifies that every value in expectedItems, exists in a grid row belonging to the grid specified by gridName
* @param {string} gridName 
* @param {Array<string>} expectedItems - a row of items we expect to find in the grid
*/
function verifyGridData(gridName, expectedItems) {

    var allItemsFound = false;
    //iterate grid rows
    getGridForVariable(gridName).store.data.items.each(function (item) {
        var expectedItemFoundCount = 0;
        //iterate expected items
        expectedItems.each(function (expectedItem) {
            for (let property in item.raw) {
                if (item.raw.hasOwnProperty(property)) {
                    if (item.raw[property].toString().startsWith(expectedItem))
                        expectedItemFoundCount++;
                }
            }
        });
        if (expectedItemFoundCount === expectedItems.length) {
            allItemsFound = true;
            return;
        }
    });
    return allItemsFound;
}