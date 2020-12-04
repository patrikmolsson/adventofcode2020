const fs = require('fs');

enum Tile {
    Square = 1,
    Tree
}

(function main() {
    const passports = inputToPassportLines();

    let validCount = 0;
    for (const passport of passports) {
        if (isValid(passport)) {
            console.log('passport is valid');
            validCount += 1;
            continue;
        }
        console.log('passport is invalid');
    }

    console.log(validCount);
})();

function isValid(passport: string) {
    const entries = passportToEntries(passport);

    if(!hasRequiredEntries(entries)) {
        return false;
    }

    return entries.every(validateEntry);
}

function validateEntry(entry: string) {
    const [key, value] = entry.split(':');

    console.log(key, value);

    switch (key) {
        case 'byr': return value >= '1920' && value <= '2002';
        case 'iyr': return value >= '2010' && value <= '2020';
        case 'eyr': return value >= '2020' && value <= '2030';
        case 'hgt': {
            const no = Number(value.replace(/\D/g, ''));

            console.log(no);

            if (value.indexOf('cm') > -1) {
                return no >= 150 && no <= 193;
            } else {
                return no >= 59 && no <= 76;
            }
        }
        case 'hcl': return value.match(/^#[0-9a-f]{6}$/);
        case 'ecl': return ['amb', 'blu', 'brn', 'gry', 'grn', 'hzl', 'oth'].includes(value);
        case 'pid': return value.match(/^[0-9]{9}$/);
        case 'cid': return true;
        default: {
            throw new Error('Unknown key');
        }
    }
}

function hasRequiredEntries(entries: string[]) {
    if (entries.length === 7 && !entries.some(entry => entry.indexOf('cid:') === 0)) {
        return true;
    } else if (entries.length === 8) {
        return true;
    }

    return false;
}

function passportToEntries(passport: string) {
    return passport.split(' ');
}

function inputToPassportLines() {
    const lines = fs.readFileSync('./input.txt').toString('utf-8');

    return lines.split('\r\n\r\n').map(p => p.replace(/\r\n/g, ' '));
}