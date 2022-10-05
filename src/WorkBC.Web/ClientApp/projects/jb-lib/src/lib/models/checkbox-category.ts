export interface CheckboxCategory {
  obj: string;
  prefix: string;
  key: string;
  listName?: string;
  listValueField?: string;
  filters: CheckboxFilter[]
}

export interface CheckboxFilter {
  id: string;
  key: string;
  label: string
}
