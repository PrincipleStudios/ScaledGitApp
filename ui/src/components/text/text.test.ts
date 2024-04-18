import { describe } from 'vitest';
import { describeElementTemplate } from '../templating/test-utils';
import { Prose, HintText } from './text';

describe('text components', () => {
	describeElementTemplate(Prose);
	describeElementTemplate(HintText);
});
