import { describe } from 'vitest';
import { describeElementTemplate } from '../templating/test-utils';
import { H1, H2, H3, H4, H5, H6 } from './headings';

describe('headings', () => {
	describeElementTemplate(H1);
	describeElementTemplate(H2);
	describeElementTemplate(H3);
	describeElementTemplate(H4);
	describeElementTemplate(H5);
	describeElementTemplate(H6);
});
