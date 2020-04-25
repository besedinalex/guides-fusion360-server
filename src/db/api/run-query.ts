import db from './sqlite3-connection';

/**
 * Detects type of passed query.
 * @param sqlQuery SQL query.
 * @returns Type of SQL query in lower case (select, update, ...)
 */
function detectQueryType(sqlQuery: string): string {
    return sqlQuery.split(' ')[0].toLowerCase();
}

/**
 * Selects data from database.
 * @param sqlQuery {string} SQL query to execute. Expects SELECT query.
 * @param firstValueOnly {boolean} By default returns array of objects. Pass 'true' to return first element only.
 * @returns {Promise<any>} Returns array of data or data object if you request for first value only.
 * Returns undefined if request isn't SQL SELECT.
 */
function selectData(sqlQuery: string, firstValueOnly = false): Promise<any> | void {
    if (detectQueryType(sqlQuery) !== 'select') {
        throw Error('selectData() expects sql SELECT query');
    }
    return new Promise((resolve, reject) => {
        db.all(sqlQuery, [], function (err, rows) {
            if (err) {
                reject(err);
            } else {
                if (firstValueOnly) {
                    if (rows.length === 0) {
                        reject(err);
                    } else {
                        resolve(rows[0]);
                    }
                } else {
                    resolve(rows);
                }
            }
        });
    });
}

/**
 * Insert, update or delete data.
 * @param sqlQuery SQL query to execute. Expects INSERT, UPDATE or DELETE or CREATE query.
 * @returns Returns id of inserted item on SQL INSERT. Returns 1 on anything else.
 * Returns undefined if request is not expected by the function.
 */
function changeData(sqlQuery: string): Promise<number> | void {
    const queryType = detectQueryType(sqlQuery);
    if (queryType !== 'insert' && queryType !== 'update' && queryType !== 'delete' && queryType !== 'create') {
        throw Error('changeData() expects sql INSERT or UPDATE or DELETE or CREATE query');
    }
    return new Promise((resolve, reject) => {
        db.run(sqlQuery, [], function (err) {
            if (err) {
                reject(err);
            } else if (queryType === 'insert') {
                resolve(this.lastID);
            } else { // update or delete
                resolve(this.changes);
            }
        })
    });
}

export {
    selectData,
    changeData
}
