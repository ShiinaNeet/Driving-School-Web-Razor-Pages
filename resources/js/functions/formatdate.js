import moment from 'moment';
export default function(date, format, other) {
    return date ? moment(date).format(format) : other;
}
